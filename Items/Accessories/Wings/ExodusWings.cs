using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Armor.Empyrean;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class ExodusWings : BaseWings
    {
        public override float BonusAscentWhileFalling => 0.85f;
        public override float BonusAscentWhileRising => 0.15f;
        public override float RisingSpeedThreshold => 1f;
        public override float MaxAscentSpeed => 3f;
        public override float BaseAscent => 0.135f;

        public override void SetStaticDefaults() => ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f);

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.armor[0].type == ModContent.ItemType<EmpyreanMask>() && player.armor[1].type == ModContent.ItemType<EmpyreanCloak>() && player.armor[2].type == ModContent.ItemType<EmpyreanCuisses>())
            {
                player.AddBuff(ModContent.BuffType<EmpyreanWrath>(), 2);
            }

            if (player.wingTime > 0f && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                Vector2 spawnPos = player.Center + new Vector2(-25 * player.direction, 0) + Main.rand.NextVector2Circular(20, 20);
                Vector2 spawnPos2 = player.Center + new Vector2(15 * player.direction, 0) + Main.rand.NextVector2Circular(20, 20);

                float partScale = Main.rand.NextFloat(0.3f, 0.8f);
                Vector2 partVel = new Vector2(0, 5).RotatedBy(0.5f * player.direction).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 0.8f);

                Particle smoke = new HeavySmokeParticle(spawnPos, partVel, Color.Black, 13, partScale * 0.9f, 0.7f, Main.rand.NextFloat(-0.2f, 0.2f), false);
                GeneralParticleHandler.SpawnParticle(smoke);
                
                if (Main.rand.NextBool((player.controlJump ? 2 : 4)))
                {
                    Dust dust = Dust.NewDustPerfect(spawnPos, ModContent.DustType<VoidDustInverted>(), partVel, 0, default, partScale * 2f);
                    dust.noGravity = true;
                    dust.color = Color.LightGreen;

                }
                if (Main.rand.NextBool())
                {
                    Particle smoke2 = new HeavySmokeParticle(spawnPos2, partVel, Color.Black, 13, partScale * 0.6f, 0.7f, Main.rand.NextFloat(-0.2f, 0.2f), false);
                    GeneralParticleHandler.SpawnParticle(smoke2);

                    if (Main.rand.NextBool((player.controlJump ? 2 : 4)))
                    {
                        Dust dust = Dust.NewDustPerfect(spawnPos2, ModContent.DustType<VoidDustInverted>(), partVel, 0, default, partScale * 1.7f);
                        dust.noGravity = true;
                        dust.color = Color.LightGreen;
                    }
                }

            }
            player.wingTimeMax = 180;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofFlight, 20).
                AddIngredient<MeldBlob>(14).
                AddIngredient(ItemID.LunarBar, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
