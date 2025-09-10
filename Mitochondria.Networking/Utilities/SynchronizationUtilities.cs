using System.Buffers.Binary;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Mitochondria.Networking.Utilities;

public static class SynchronizationUtilities
{
    public static DateTimeOffset NtpUtcNow => DateTimeOffset.UtcNow + _offsetFromSystemUtcTime;

    private static TimeSpan _offsetFromSystemUtcTime = TimeSpan.Zero;

    public static void ResyncNtpUtcNow()
    {
        // Credit: https://stackoverflow.com/a/12150289/

        // TODO: Update to add era handling support before 2036.

        const string ntpServer = "time.cloudflare.com";
        const int transitTimestampOffset = 40;

        IPAddress address;
        try
        {
            address = Dns.GetHostAddresses(ntpServer, AddressFamily.InterNetwork).Random() ??
                      throw new Exception($"Resolving \"{ntpServer}\" returned no IP addresses.");
        }
        catch (Exception e)
        {
            Error($"Failed to get NTP time: {e}");
            return;
        }

        var endPoint = new IPEndPoint(address, 123);

        var requestData = (Span<byte>) stackalloc byte[48];
        requestData[0] = 0x1B;

        Span<byte> responseData;
        try
        {
            using var udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 3000;

            udpClient.Send(requestData, endPoint);

            var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            var failedAttempts = 0;
            while (true)
            {
                if (failedAttempts >= 3)
                {
                    Error("Received no data from the NTP server after 3 attempts.");
                    return;
                }

                try
                {
                    responseData = udpClient.Receive(ref remoteEndPoint);
                    break;
                }
                catch
                {
                    failedAttempts++;
                }
            }
        }
        catch (Exception e)
        {
            Error($"Failed to send/receive data to/from the NTP server: {e}");
            return;
        }

        var systemUtcTime = DateTimeOffset.UtcNow;

        var integerPart = (ulong) BinaryPrimitives.ReadUInt32BigEndian(responseData[transitTimestampOffset..]);
        var fractionPart = (ulong) BinaryPrimitives.ReadUInt32BigEndian(responseData[(transitTimestampOffset + 4)..]);

        var milliseconds = integerPart * 1000 + fractionPart * 1000 / 0x100000000L;

        var transitTimestamp = new DateTimeOffset(1900, 1, 1, 0, 0, 0, TimeSpan.Zero)
            .AddMilliseconds(milliseconds);

        _offsetFromSystemUtcTime = transitTimestamp - systemUtcTime;
    }

    internal class NtpSynchronizeBehaviour : MonoBehaviour
    {
        public NtpSynchronizeBehaviour(IntPtr ptr) : base(ptr) { }

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private void Awake()
            => StartCoroutine(CoKeepNtpSynchronised().WrapToIl2Cpp());

        private void OnDestroy()
            => _cancellationTokenSource.Cancel();

        private IEnumerator CoKeepNtpSynchronised()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                ResyncNtpUtcNow();

                yield return new WaitForSeconds((float) TimeSpan.FromMinutes(5).TotalSeconds);
            }
        }
    }
}
