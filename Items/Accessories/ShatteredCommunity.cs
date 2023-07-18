using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Accessories
{
    public class ShatteredCommunity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        // The percentage of a full Rage bar that is gained every second with the Shattered Community equipped.
        public const float RagePerSecond = 0.02f;

        // It is pretty hard to have less than 10 iframes for any reason, but this is a stopgap measure against Rage abuse.
        public static readonly int RageGainCooldown = 10;

        private static readonly Color rarityColorOne = new Color(128, 62, 128);
        private static readonly Color rarityColorTwo = new Color(245, 105, 245);

        // Base level cost is 400,000 damage dealt while Rage is active.
        // Each successive level costs (400,000 * level) MORE damage, so the total required goes up quadratically.
        // Total required to reach level 60 is 732,000,000 damage dealt.
        // This is within one order of magnitude of the integer limit, so relevant values are stored as longs.
        internal const long BaseLevelCost = 400000L;
        internal static long LevelCost(int level) => BaseLevelCost * level;
        internal static long CumulativeLevelCost(int level) => (BaseLevelCost / 2L) * level * (level + 1);
        internal const int MaxLevel = 25; // was 60.
        internal const float RageDamagePerLevel = 0.01f; // x1.35 --> x1.60 (used to be x1.95, jesus)

        internal int level = 0;
        internal long totalRageDamage = 0L;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        // Not overriding these Clones makes tooltips fail to function correctly due to HoverItem spaghetti.
        public override ModItem Clone(Item item)
        {
            var clone = (ShatteredCommunity)base.Clone(Item);
            clone.level = level;
            clone.totalRageDamage = totalRageDamage;
            return clone;
        }

        internal static Color GetRarityColor() => CalamityUtils.ColorSwap(rarityColorOne, rarityColorTwo, 3f);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.shatteredCommunity = true;
            ShatteredCommunityPlayer scp = player.GetModPlayer<ShatteredCommunityPlayer>();
            scp.sc = this;

            // Shattered Community gives (mostly) the same boosts as normal Community
            // It does not give melee speed or minion knockback, but always gives life regen (instead of only conditionally)
            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.GetCritChance<GenericDamageClass>() += 5;
            player.statDefense += 10;
            player.endurance += 0.05f;
            player.lifeRegen += 2;
            player.moveSpeed += 0.1f;

            // Shattered Community provides a stacking +1% Rage Mode damage per level.
            modPlayer.RageDamageBoost += level * RageDamagePerLevel;
        }

        // Community and Shattered Community are mutually exclusive
        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().community;
        public override bool CanUseItem(Player player) => false;

        // Produces purple light while in the world.
        public override void PostUpdate()
        {
            float brightness = Main.essScale;
            Lighting.AddLight(Item.Center, 0.92f * brightness, 0.42f * brightness, 0.92f * brightness);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Add the proper description which changes depending on world difficulty
            string desc = CalamityWorld.revenge ? this.GetLocalizedValue("RageModified") : this.GetLocalization("RageAdd").Format(CalamityKeybinds.RageHotKey.TooltipHotkeyString());
            tooltips.FindAndReplace("[RAGEDESC]", desc);

            // Add the current level
            tooltips.FindAndReplace("[LEVEL]", level.ToString());

            // Add the progress
            string progressKey = "[PROGRESS]";
            TooltipLine progressLine = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Text.Contains(progressKey));
            if (progressLine != null)
            {
                if (level < MaxLevel)
                {
                    long progressToNextLevel = totalRageDamage - CumulativeLevelCost(level);
                    long totalToNextLevel = LevelCost(level + 1);
                    double ratio = (double)progressToNextLevel / totalToNextLevel;
                    string percent = (100D * ratio).ToString("0.00");
                    progressLine.Text = progressLine.Text.Replace(progressKey, percent);
                }
                else
                    progressLine.Text = string.Empty;
            }

            // Add the current cumulative damage
            tooltips.FindAndReplace("[DAMAGE]", totalRageDamage.ToString());
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("level", level);
            tag.Add("totalDamage", totalRageDamage);
        }

        public override void LoadData(TagCompound tag)
        {
            level = tag.GetInt("level");
            // Shattered Community's level cap was reduced from 60 to 25, so cap out ones that were made higher previously.
            if (level > MaxLevel)
                level = MaxLevel;
            totalRageDamage = tag.GetLong("totalDamage");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(level);
            writer.Write(totalRageDamage);
        }

        public override void NetReceive(BinaryReader reader)
        {
            level = reader.ReadInt32();
            totalRageDamage = reader.ReadInt64();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.5f,
                drawOffset: new(0f, 2f)
            );
            //return false;
            return true; // For some reason, putting this item in an accessory slot hides it UNLESS this returns true.
        }
    }

    public class ShatteredCommunityPlayer : ModPlayer
    {
        internal ShatteredCommunity sc = null;

        public override void ResetEffects() => sc = null;

        internal void AccumulateRageDamage(long damage)
        {
            if (sc is null)
                return;

            // Actually accumulate the damage.
            sc.totalRageDamage += damage;

            // Level up if applicable.
            if (sc.level < ShatteredCommunity.MaxLevel && sc.totalRageDamage > ShatteredCommunity.CumulativeLevelCost(sc.level + 1))
            {
                ++sc.level;
                LevelUpEffects(sc.Item);
            }
        }

        private void LevelUpEffects(Item item)
        {
            // Spawn the purple laser beam from failing the Dungeon Defenders event.
            var source = Player.GetSource_Accessory(item);
            int projID = ProjectileID.DD2ElderWins;
            Vector2 offset = new Vector2(0f, 800f); // The effect is extremely tall, so start it very low down
            Projectile fx = Projectile.NewProjectileDirect(source, Player.Center + offset, Vector2.Zero, projID, 0, 0f, Player.whoAmI);
            fx.friendly = false;
            fx.hostile = false;
            // On the 108th update, crystal debris is spawned, so we avoid that.
            fx.timeLeft = 107;
            fx.MaxUpdates = 2; // Make the animation play at double speed.

            // Play a weird dimensional lightning sound simultaneously.
            var extraSound = SoundID.DD2_EtherianPortalDryadTouch with { Volume = SoundID.DD2_EtherianPortalDryadTouch.Volume * 1.4f };
            SoundEngine.PlaySound(extraSound, Player.Center);

            // Display a level up text notification.
            Rectangle textArea = new Rectangle((int)Player.Center.X, (int)Player.Center.Y, 1, 1);
            Color textColor = new Color(236, 209, 236);
            CombatText.NewText(textArea, textColor, CalamityUtils.GetTextValueFromModItem<ShatteredCommunity>("LevelUpText"), false, false);
        }
    }
}
