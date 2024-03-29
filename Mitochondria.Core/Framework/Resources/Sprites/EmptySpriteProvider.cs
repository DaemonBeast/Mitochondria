﻿using UnityEngine;

namespace Mitochondria.Core.Framework.Resources.Sprites;

public class EmptySpriteProvider : SpriteProvider
{
    private Sprite? _sprite;
    
    public override Sprite Load(bool useCached = true)
    {
        return useCached ? _sprite ??= CreateEmptySprite() : _sprite = CreateEmptySprite();
    }

    private static Sprite CreateEmptySprite()
    {
        return SpriteHelper.CreateSprite(SpriteHelper.CreateEmptyTexture(), 100f);
    }
}