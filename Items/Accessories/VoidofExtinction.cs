using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VoidofExtinction : ModItem
    {
        public const int FireProjectiles = 2;
        public const float FireAngleSpread = 120;
        public int FireCountdown = 0;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Void of Extinction");
            Tooltip.SetDefault("Drops brimstone fireballs from the sky occasionally\n" +
                "10% increase to all damage\n" +
                "All attacks inflict Brimstone Flames\n" +
                "Brimstone fire rains down after getting hit\n" +
                "Reduces damage from touching lava\n" +
                "Grants immunity to Burning, On Fire!, Brimstone Flames and Searing Lava");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
            Item.defense = 8;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded) => !player.Calamity().voidOfCalamity;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenSkullRose).
                AddIngredient<Gehenna>().
                AddIngredient<Abaddon>().
                AddIngredient<VoidofCalamity>().
                AddIngredient<CoreofChaos>().
                AddIngredient<ScoriaBar>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var source = player.GetSource_Accessory(Item);
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.voidOfCalamity = true;
            modPlayer.voidOfExtinction = true;
            modPlayer.abaddon = true;
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.fireWalk = true;
            player.lavaRose = true;
            player.GetDamage<GenericDamageClass>() += 0.1f;
            if (player.immune)
            {
                if (player.miscCounter % 10 == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int damage = (int)player.GetBestClassDamage().ApplyTo(30);
                        Projectile fire = CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<StandingFire>(), damage, 5f, player.whoAmI);
                        if (fire.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            fire.usesLocalNPCImmunity = true;
                            fire.localNPCHitCooldown = 60;
                        }
                    }
                }
            }
            if (FireCountdown == 0)
            {
                FireCountdown = 600;
            }
            if (FireCountdown > 0)
            {
                FireCountdown--;
                if (FireCountdown == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int projSpeed = 25;
                        float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
                        float spawnY = -1000 + player.Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX, spawnY);
                        Vector2 baseVelocity = player.Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= projSpeed;
                        for (int i = 0; i < FireProjectiles; i++)
                        {
                            Vector2 spawn = baseSpawn;
                            spawn.X += i * 30 - (FireProjectiles * 15);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-FireAngleSpread / 2 + (FireAngleSpread * i / (float)FireProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            int damage = (int)player.GetBestClassDamage().ApplyTo(70);
                            Projectile.NewProjectile(source, spawn, velocity, ModContent.ProjectileType<BrimstoneHellfireballFriendly2>(), damage, 5f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
        }
    }
}
