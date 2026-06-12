using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.CalPlayer;
using CalamityMod.Items.BaseItems;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;

namespace CalamityMod.Items.Accessories.Vanity
{
    public class XyksBlessingBlue : TransformationAccessory, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        int itemFrame = 0;
        int itemFrameTimer = 1;
        int pulseTimer = 0;
        public static Vector2 extraPosition = Vector2.Zero;
        public static Color baseEffectColor = new Color(150, 255, 210); // Teal-ish Glow
        public static Color animEffectColor = new Color(255, 85, 252); // Purple Flash
        public static Color baseMainColor = new Color(25, 118, 218); // Mid Blue
        public static Color baseAccentColor = new Color(0, 255, 250); // Light blue
        public override (EquipType, string, string)[] EquipSlots =>
        [
            (EquipType.Head, "Xyk", null),
            (EquipType.Body, "Xyk", null),
            (EquipType.Legs, "Xyk", null),
            (EquipType.Wings, null, null), //results in setting this equip slot to -1
        ];
        public static Asset<Texture2D> Texture = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/Vanity/XyksBlessingBlueAnim");
        public static Asset<Texture2D> AltTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/Vanity/XyksBlessingBlueAnim2");
        public static Asset<Texture2D> ShapeTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomRing");
        public static Asset<Texture2D> BloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");

        bool NoArmorEnabled = false;
        bool DoTransformAnimation = false;
        float effectsMult = 1;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("[TOGGLE]", NoArmorEnabled ? this.GetLocalizedValue("ArmorDisabled") : this.GetLocalizedValue("ArmorEnabled"));
        }
        public override bool ConsumeItem(Player player) => !Main.keyState.PressingShift();
        public override void SaveData(TagCompound tag)
        {
            tag.Add("noArmor", NoArmorEnabled);
            tag.Add("doAnim", DoTransformAnimation);
        }
        public override void LoadData(TagCompound tag)
        {
            NoArmorEnabled = tag.GetBool("noArmor");
            DoTransformAnimation = tag.GetBool("doAnim");
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(NoArmorEnabled);
            writer.Write(DoTransformAnimation);
        }
        public override void NetReceive(BinaryReader reader)
        {
            NoArmorEnabled = reader.ReadBoolean();
            DoTransformAnimation = reader.ReadBoolean();
        }
        public Rectangle GetFrame()
        {
            Texture2D texture = DoTransformAnimation ? AltTexture.Value : Texture.Value;
            int vertialFramesNum = DoTransformAnimation ? 13 : 20;
            Rectangle frame = texture.Frame(verticalFrames: vertialFramesNum, frameY: itemFrame);
            return frame;
        }
        public void DrawItem(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, float scale, float rotation, bool inWorld)
        {
            itemFrameTimer++;
            if (itemFrameTimer % 5 == 0)
                itemFrame++;

            effectsMult = MathHelper.Lerp(effectsMult, 1, 0.1f);
            Color effectColor = Color.Lerp(baseAccentColor, animEffectColor, Math.Max(effectsMult - 1, 0));

            Texture2D texture = DoTransformAnimation ? AltTexture.Value : Texture.Value;
            float sine = MathF.Sin(Main.GlobalTimeWrappedHourly * 5);
            extraPosition = new Vector2(0, 5 * (inWorld ? 1 : 0.2f) * sine);
            Vector2 animPlaceAdjust = Vector2.UnitY * (DoTransformAnimation ? 2.8f : 0);

            float pulseProgress = MathF.Pow(Utils.GetLerpValue(90, 0, pulseTimer % 90), 3.5f);

            if (itemFrame >= (DoTransformAnimation ? 13 : 20))
            {
                itemFrame = (DoTransformAnimation ? 8 : 0);
                if (DoTransformAnimation) { DoTransformAnimation = false; Item.NetStateChanged(); }
            }
            //Pulse
            Texture2D pulse = ShapeTexture.Value;
            spriteBatch.Draw(pulse, position + extraPosition, null, Color.Lerp(effectColor, baseEffectColor, pulseProgress) with { A = 0 } * pulseProgress, rotation, pulse.Size() / 2, scale * (1 - pulseProgress) * effectsMult, SpriteEffects.None, 0f);

            Texture2D bloom = BloomTexture.Value;
            Vector2 squash = new Vector2(1 + Math.Max(effectsMult - 1, 0) * 3.5f, 0.85f - Math.Max(effectsMult - 1, 0) * 0.75f);
            float bloomScale = 0.75f;
            // Bloom effects
            spriteBatch.Draw(bloom, position + extraPosition, null, effectColor with { A = 0 }, rotation, bloom.Size() / 2, new Vector2(1, 0.85f) * (scale + 0.05f * sine) * bloomScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bloom, position + extraPosition, null, effectColor with { A = 0 }, rotation, bloom.Size() / 2, squash * (scale + 0.05f * sine) * 0.9f * bloomScale, SpriteEffects.None, 0f);
            // Sprite
            spriteBatch.Draw(texture, position + extraPosition + animPlaceAdjust, frame, (inWorld ? Lighting.GetColor((Item.Center + extraPosition + animPlaceAdjust).ToTileCoordinates()) : Color.White), 0f, frame.Size() / 2, scale, SpriteEffects.None, 0f);
            pulseTimer++;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            frame = GetFrame();
            DrawItem(spriteBatch, position, frame, scale * 1.5f, 0, false);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rectangle itemFrame = GetFrame();
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, (itemFrame.Size() / 2).Y);
            DrawItem(spriteBatch, drawPosition, itemFrame, scale * 0.5f, rotation, true);
            return false;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float sine = MathF.Sin(Main.GlobalTimeWrappedHourly * 3);
            Color clr = Color.Lerp(baseEffectColor, baseMainColor, Utils.GetLerpValue(-1, 1, sine));
            Lighting.AddLight(Item.Center + extraPosition, clr.ToVector3() * 0.55f);
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 25;
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.Calamity().devItem = true;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            if (Main.keyState.PressingShift())
            {
                SoundStyle sound = new("CalamityMod/Sounds/Item/StarfleetFire");
                for (int i = 0; i < 2; i++)
                    SoundEngine.PlaySound(sound with { Volume = 0.65f, Pitch = Main.rand.NextFloat(0.4f, 0.5f) + 0.9f * i, MaxInstances = 6 }, player.Center);

                effectsMult = 2;
                pulseTimer = 20;
                itemFrameTimer = 1;
                itemFrame = 0;
                NoArmorEnabled = !NoArmorEnabled;
                DoTransformAnimation = true;
                Item.NetStateChanged();
            }
            else
                player.PutItemInInventoryFromItemUsage(ModContent.ItemType<XyksBlessingOrange>());
        }
        public override Func<Player, bool> ShouldTransform => ((player) => !NoArmorEnabled);
        public override void UpdateVanity(Player player)
        {
            EquipEffects(player);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                EquipEffects(player);
            }
        }
        public void EquipEffects(Player player)
        {
            player.DisableWingFlapSound();
            if (NoArmorEnabled)
                IsForced = false;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.XykVisualsBlue = true;
        }
        // Also research the other variant
        public override void OnResearched(bool fullyResearched)
        {
            if (fullyResearched)
            {
                if (!Main.ServerSideCharacter)
                {
                    Main.LocalPlayerCreativeTracker.ItemSacrifices.RegisterItemSacrifice(ModContent.ItemType<XyksBlessingOrange>(), 1);
                }
                else
                {
                    NetPacket packet = NetCreativeUnlocksPlayerReportModule.SerializeSacrificeRequest(ModContent.ItemType<XyksBlessingOrange>(), 1);
                    NetManager.Instance.SendToServerOrLoopback(packet);
                }
            }
        }
    }
}
