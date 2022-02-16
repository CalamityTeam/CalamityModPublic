using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using CalamityMod.CalPlayer;

namespace CalamityMod.UI.CooldownIndicators
{

    public class PotionSicknessDisplay : CooldownIndicator
    {
        public override bool DisplayMe => CalamityConfig.Instance.VanillaCooldownDisplay;
        public override string Name => "Healing Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PotionSickness";
        public override Color OutlineColor => new Color(255, 142, 165);
        public override Color CooldownColorStart => Color.Lerp(new Color(208, 234, 255), new Color(231, 3, 54), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(208, 234, 255), new Color(231, 3, 54), Completion);


        public PotionSicknessDisplay(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class NebulousCoreCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true; 
        public override string Name => "Nebulous Core Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/NebulousCore";
        public override Color OutlineColor => Color.Lerp(new Color(252, 109, 203), new Color(58, 91, 146), Completion);
        public override Color CooldownColorStart => new Color(148, 62, 216);
        public override Color CooldownColorEnd => new Color(255, 187, 207);

        public NebulousCoreCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class PermafrostConcoctionCooldown : CooldownIndicator
    {
        public override string SyncID => "ConcoctionCooldown";
        public override bool DisplayMe => true;
        public override string Name => "Permafrost's Concoction Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/PermafrostConcoction";
        public override Color OutlineColor => new Color(0, 218, 255);
        public override Color CooldownColorStart => new Color(144, 184, 205);
        public override Color CooldownColorEnd => new Color(232, 246, 254);

        public PermafrostConcoctionCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class GlobalDodgeCooldown : CooldownIndicator
    {
        public override string SyncID => "GlobalDodge";
        public override bool DisplayMe => true;
        public override string Name => "Dodge Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownColorStart => cooldownColorStart;
        public override Color CooldownColorEnd => cooldownColorEnd;

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public GlobalDodgeCooldown(int duration, Player player, string skin = "default") : base(duration, player)
        {
            switch (skin)
            {
                case "abyssmirror":
                    skinTexture = "AbyssEvade";
                    outlineColor = new Color(125, 157, 149);
                    cooldownColorStart = new Color(167, 147, 151);
                    cooldownColorEnd = new Color(217, 209, 195);
                    break;

                case "eclipsemirror":
                    skinTexture = "EclipseEvade";
                    outlineColor = new Color(152, 206, 248);
                    cooldownColorStart = new Color(255, 192, 71);
                    cooldownColorEnd = new Color(255, 255, 151);
                    break;

                default:
                    skinTexture = "GlobalDodge";
                    outlineColor = Color.White;
                    cooldownColorStart = Color.CornflowerBlue;
                    cooldownColorEnd = Color.White;
                    break;
            }
        }
    }

    public class CounterScarfCooldown : CooldownIndicator
    {
        public override string SyncID => "CounterScarf";
        public override bool DisplayMe => true;
        public override string Name => "Scarf Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/CounterScarf";
        public override Color OutlineColor => Color.Lerp(new Color(255, 115, 178), new Color(255, 76, 76), (float)Math.Sin(Main.GlobalTime * 2f) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(194, 75, 97);
        public override Color CooldownColorEnd => new Color(255, 76, 76);


        public CounterScarfCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class EvasionScarfCooldown : CooldownIndicator
    {
        public override string SyncID => "EvasionScarf";
        public override bool DisplayMe => true;
        public override string Name => "Scarf Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/EvasionScarf";
        public override Color OutlineColor => Color.Lerp(new Color(255, 194, 150), new Color(255, 160, 150), (float)Math.Sin(Main.GlobalTime * 2f) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(132, 23, 32);
        public override Color CooldownColorEnd => new Color(164, 52, 45);

        public EvasionScarfCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class IceShieldCooldown : CooldownIndicator
    {
        public override string SyncID => "SirenHeartShield";
        public override bool DisplayMe => true;
        public override string Name => "Ice Shield Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SirenIceShield";
        public override Color OutlineColor => Color.Lerp(new Color(163, 186, 198), new Color(146, 187, 255), (float)Math.Sin(Main.GlobalTime) * 0.5f + 0.5f);
        public override Color CooldownColorStart => new Color(124, 195, 214);
        public override Color CooldownColorEnd => new Color(147, 230, 253);


        public IceShieldCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class GodslayerDashCooldown : CooldownIndicator
    {
        public override string SyncID => "GodslayerDash";
        public override bool DisplayMe => true;
        public override string Name => "Godslayer Dash Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/GodslayerDash";
        public override Color OutlineColor => Color.Lerp(new Color(173, 66, 203), new Color(252, 109, 202), Completion);
        public override Color CooldownColorStart => new Color(252, 109, 202);
        public override Color CooldownColorEnd => new Color(119, 254, 254);


        public GodslayerDashCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class SilvaReviveCooldown : CooldownIndicator
    {
        public override bool DisplayMe => true;
        public override string Name => "Silva Revive Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SilvaRevive";
        public override Color OutlineColor => new Color(151, 211, 152);
        public override Color CooldownColorStart => new Color(226, 188, 74);
        public override Color CooldownColorEnd => new Color(151, 211, 152);

        public override bool CanTickDown => !CalamityPlayer.areThereAnyDamnBosses && !CalamityPlayer.areThereAnyDamnEvents;

        public SilvaReviveCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }
    
    public class SandCloakCooldown : CooldownIndicator
    {
        public override string SyncID => "SandCloak";
        public override bool DisplayMe => true;
        public override string Name => "Sand Cloak Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/SandCloak";
        public override Color OutlineColor => new Color(209, 176, 114);
        public override Color CooldownColorStart => new Color(100, 64, 44);
        public override Color CooldownColorEnd => new Color(132, 95, 54);


        public SandCloakCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    //We may want an "alt skin" for when its trigged by cotbg
    public class FleshTotemCooldown : CooldownIndicator
    {
        public override string SyncID => "FleshTotem";
        public override bool DisplayMe => true;
        public override string Name => "Contact Damage Halving Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/" + skinTexture;
        public override Color OutlineColor => outlineColor;
        public override Color CooldownColorStart => cooldownColorStart;
        public override Color CooldownColorEnd => cooldownColorEnd;

        //It's the same cooldown with different skins each time, basically.
        public string skinTexture;
        public Color outlineColor;
        public Color cooldownColorStart;
        public Color cooldownColorEnd;

        public FleshTotemCooldown(int duration, Player player, string skin = "default") : base(duration, player)
        {
            switch (skin)
            {
                case "bloodgod":
                    skinTexture = "BloodGodTotem";
                    outlineColor = new Color(255, 162, 205);
                    cooldownColorStart = new Color(193, 205, 255);
                    cooldownColorEnd = new Color(255, 193, 219);
                    break;

                default:
                    skinTexture = "FleshTotem";
                    outlineColor = new Color(157, 248, 234);
                    cooldownColorStart = new Color(111, 169, 241);
                    cooldownColorEnd = new Color(111, 169, 241);
                    break;
            }
        }
    }

    public class InkBombCooldown : CooldownIndicator
    {
        public override string SyncID => "InkBomb";
        public override bool DisplayMe => true;
        public override string Name => "Ink Bomb Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/InkBomb";
        public override Color OutlineColor => new Color(205, 182, 137);
        public override Color CooldownColorStart => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(177, 147, 89), new Color(105, 103, 126), Completion);


        public InkBombCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class LionsHeartShieldCooldown : CooldownIndicator
    {
        public override string SyncID => "LionsHeartShield";
        public override bool DisplayMe => true;
        public override string Name => "Energy Shell Cooldown";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/LionHeartShield";
        public override Color OutlineColor => new Color(232, 239, 239);
        public override Color CooldownColorStart => new Color(17, 242, 244);
        public override Color CooldownColorEnd => Color.White;

        public LionsHeartShieldCooldown(int duration, Player player) : base(duration, player)
        {
        }
    }

    public class DivingBrokenPlatesCooldown : CooldownIndicator
    {
        public override string SyncID => "AbyssalDivingBrokenPlates";
        public override bool DisplayMe => true;
        public override string Name => "Abyssal Diving Suit Broken Plates";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BrokenPlates";
        public override Color OutlineColor => new Color(194, 173, 146);
        public override Color CooldownColorStart => Color.Lerp(new Color(91, 121, 150), new Color(30, 50, 77), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(91, 121, 150), new Color(30, 50, 77), Completion);


        public DivingBrokenPlatesCooldown(int duration, Player player) : base(duration, player)
        {
        }

        public override void OnCooldownEnd()
        {
            AfflictedPlayer.Calamity().abyssalDivingSuitPlateHits = 0;
        }
    }

    public class DivingPlatesBreaking : CooldownIndicator
    {
        public override bool DisplayMe => AfflictedPlayer.Calamity().abyssalDivingSuit;
        public override string Name => "Abyssal Diving Suit Plates Durability";
        public override string Texture => "CalamityMod/UI/CooldownIndicators/BreakingPlates";
        public override string TextureOutline => "CalamityMod/UI/CooldownIndicators/BrokenPlatesOutline";
        public override string TextureOverlay => "CalamityMod/UI/CooldownIndicators/BrokenPlatesOverlay";
        public override Color OutlineColor => TimeLeft == 0 ? new Color(147, 218, 183) : TimeLeft == 1 ? new Color(233, 190, 134) : new Color(220, 111, 94);
        public override Color CooldownColorStart => Color.Lerp(new Color(160, 174, 174), new Color(192, 11, 107), Completion);
        public override Color CooldownColorEnd => Color.Lerp(new Color(160, 174, 174), new Color(192, 11, 107), Completion);
        public override bool CanTickDown => !AfflictedPlayer.Calamity().abyssalDivingSuit;

        public DivingPlatesBreaking(int duration, Player player) : base(duration, player)
        {
        }

        public override bool UseCustomDrawCompact => true;

        public override void CustomDrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = GetTexture(Texture);
            Texture2D outline = GetTexture(TextureOutline);

            //Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            DrawBorderStringEightWay(spriteBatch, Main.fontMouseText, (3 - TimeLeft).ToString(), position + new Vector2(-3, -3) * scale, CooldownColorStart, Color.Black, scale);
        }
    }


    public class OmegaBlueCooldown : CooldownIndicator
    {
        public override string SyncID => "OmegaBlue";
        public override bool DisplayMe => true;
        public override string Name => TimeLeft > 1500 ? "Abyssal Madness" : "Abyssal Madness Cooldown";
        public override string Texture => TimeLeft > 1500 ? "CalamityMod/UI/CooldownIndicators/AbyssalMadnessActive" : "CalamityMod/UI/CooldownIndicators/AbyssalMadness";
        public override string TextureOutline => "CalamityMod/UI/CooldownIndicators/AbyssalMadnessOutline";
        public override string TextureOverlay => "CalamityMod/UI/CooldownIndicators/AbyssalMadnessOverlay";
        public override Color OutlineColor => TimeLeft > 1500 ? new Color(231, 164, 1) : new Color(72, 135, 205);
        public override Color CooldownColorStart => TimeLeft > 1500 ? Color.Lerp(new Color(98, 110, 179), new Color(216, 176, 80), (TimeLeft - 1500) / 300f) : new Color(98, 110, 179);
        public override Color CooldownColorEnd => TimeLeft > 1500 ? Color.Lerp(new Color(179, 132, 98), new Color(216, 176, 80), (TimeLeft - 1500) / 300f) : new Color(179, 132, 98);
        public override LegacySoundStyle EndSound => AfflictedPlayer.Calamity().mod.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/OmegaBlueRecharge");

        public OmegaBlueCooldown(int duration, Player player) : base(duration, player)
        {
        }

        public override void OnCooldownEnd()
        {
            for (int i = 0; i < 66; i++)
            {
                int d = Dust.NewDust(AfflictedPlayer.position, AfflictedPlayer.width, AfflictedPlayer.height, 20, 0, 0, 100, Color.Transparent, 2.6f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].fadeIn = 1f;
                Main.dust[d].velocity *= 6.6f;
            }
        }

        //Charge down at first, and then charge back up
        private float AdjustedCompletion => TimeLeft > 1500 ? (TimeLeft - 1500) / 300f : 1 - TimeLeft / 1500f;

        public override void ApplyBarShaders(float opacity)
        {
            //Use the adjusted completion
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(AdjustedCompletion);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownColorStart);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownColorEnd);
            GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();
        }

        public override bool UseCustomDrawCompact => true;

        public override void CustomDrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = GetTexture(Texture);
            Texture2D outline = GetTexture(TextureOutline);
            Texture2D overlay = GetTexture(TextureOverlay);

            //Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the small overlay
            int lostHeight = (int)Math.Ceiling(overlay.Height * AdjustedCompletion);
            Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
            spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}
