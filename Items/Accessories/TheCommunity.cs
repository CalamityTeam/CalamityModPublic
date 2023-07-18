using CalamityMod.CalPlayer;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace CalamityMod.Items.Accessories
{
    public class TheCommunity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 10));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 64;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Rainbow>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.community = true;
        }

        // Community and Shattered Community are mutually exclusive
        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().shatteredCommunity;

        // Returns the total power of the Community, from 0.05 to 0.2, scaling based on bosses defeated in the world.
        // Returns the percentage of bosses killed (from 0.0 to 1.0) instead if killsOnly is true
        private static readonly int TotalCountedBosses = 42;
        internal static float CalculatePower(bool killsOnly = false)
        {
            int numBosses = 0;
            numBosses += NPC.downedSlimeKing.ToInt();
            numBosses += DownedBossSystem.downedDesertScourge.ToInt();
            numBosses += NPC.downedBoss1.ToInt();
            numBosses += DownedBossSystem.downedCrabulon.ToInt();
            numBosses += NPC.downedBoss2.ToInt();
            numBosses += (DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator).ToInt();
            numBosses += NPC.downedQueenBee.ToInt();
            numBosses += NPC.downedBoss3.ToInt();
            numBosses += NPC.downedDeerclops.ToInt();
            numBosses += DownedBossSystem.downedSlimeGod.ToInt(); // 10
            numBosses += Main.hardMode.ToInt();
            numBosses += NPC.downedQueenSlime.ToInt();
            numBosses += DownedBossSystem.downedCryogen.ToInt();
            numBosses += NPC.downedMechBoss1.ToInt();
            numBosses += DownedBossSystem.downedAquaticScourge.ToInt();
            numBosses += NPC.downedMechBoss2.ToInt();
            numBosses += DownedBossSystem.downedBrimstoneElemental.ToInt();
            numBosses += NPC.downedMechBoss3.ToInt();
            numBosses += DownedBossSystem.downedCalamitasClone.ToInt();
            numBosses += NPC.downedPlantBoss.ToInt(); // 20
            numBosses += DownedBossSystem.downedLeviathan.ToInt();
            numBosses += DownedBossSystem.downedAstrumAureus.ToInt();
            numBosses += NPC.downedGolemBoss.ToInt();
            numBosses += DownedBossSystem.downedPlaguebringer.ToInt();
            numBosses += NPC.downedFishron.ToInt();
            numBosses += NPC.downedEmpressOfLight.ToInt();
            numBosses += DownedBossSystem.downedRavager.ToInt();
            numBosses += NPC.downedAncientCultist.ToInt();
            numBosses += DownedBossSystem.downedAstrumDeus.ToInt();
            numBosses += NPC.downedMoonlord.ToInt(); // 30
            numBosses += DownedBossSystem.downedGuardians.ToInt();
            numBosses += DownedBossSystem.downedDragonfolly.ToInt();
            numBosses += DownedBossSystem.downedProvidence.ToInt();
            numBosses += DownedBossSystem.downedCeaselessVoid.ToInt();
            numBosses += DownedBossSystem.downedStormWeaver.ToInt();
            numBosses += DownedBossSystem.downedSignus.ToInt();
            numBosses += DownedBossSystem.downedPolterghast.ToInt();
            numBosses += DownedBossSystem.downedBoomerDuke.ToInt();
            numBosses += DownedBossSystem.downedDoG.ToInt();
            numBosses += DownedBossSystem.downedYharon.ToInt(); // 40
            numBosses += DownedBossSystem.downedExoMechs.ToInt();
            numBosses += DownedBossSystem.downedCalamitas.ToInt();
            float bossDownedRatio = numBosses / (float)TotalCountedBosses;
            return killsOnly ? bossDownedRatio : MathHelper.Lerp(0.05f, 0.2f, bossDownedRatio);
        }

        // Damage stat boosts
        public const float DamageMultiplier = 0.5f; // 2.5% to 10% (x100)
        public const float CritMultiplier = 25f; // 1.25% to 5%

        // Defensive stat boosts
        public const float HealthMultiplier = 50f; // 2% to 10% (rounded down)
        public const float DRMultiplier = 0.25f; // 1.25% to 5% (x100)
        public const float DefenseMultiplier = 50f; // 2 to 10 (rounded down)

        // Only while affected by DoT
        public const float RegenMultiplier = 10f; // 0 to 2 (rounded down), +1 added independently of the multiplier

        // Mobility stat boosts
        public const float SpeedMultiplier = 0.5f; // 2.5% to 10% (x100)
        public const float FlightMultiplier = 1f; // 5% to 20% (x100)
        
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            float power = CalculatePower();
            string statList = this.GetLocalization("StatsList").Format(
                (DamageMultiplier * power * 100).ToString("N1"),
                (CritMultiplier * power).ToString("N0"),
                (int)(HealthMultiplier * power),
                (DRMultiplier * power * 100).ToString("N2"),
                (int)(DefenseMultiplier * power),
                1 + (int)(RegenMultiplier * power),
                (SpeedMultiplier * power * 100).ToString("N1"),
                (FlightMultiplier * power * 100).ToString("N1"),
                (CalculatePower(true) * 100).ToString("N0"));
            list.FindAndReplace("[STATS]", statList);
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
                wantedScale: 0.7f,
                drawOffset: new(0f, 0f)
            );
            return false;
        }
    }
}
