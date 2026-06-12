using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Cooldowns;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ForbiddenOathblade : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public int throwCount = 0;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<DemonicFlames>()];
        }
        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 74;
            Item.damage = 58;
            Item.crit = 21;
            Item.useAnimation = Item.useTime = 50;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<ForbiddenOathbladeThrownBlade>();
            Item.useTurn = true;
            Item.knockBack = 4.5f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.channel = true;

            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }
        public override bool MeleePrefix() => true;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<ForbiddenOathbladeHoldout>()] <= 0 && !player.Calamity().mouseRight;
        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
            if (player.whoAmI != Main.myPlayer)
                return;
            if (player.Calamity().mouseRight && !player.mouseInterface && player.Calamity().killModeCooldown == 0 && !Main.mapFullscreen && !Main.blockMouse)
            {
                SoundStyle buff = new("CalamityMod/Sounds/Item/DemonSwordKillMode");
                SoundEngine.PlaySound(buff with { Volume = 0.95f }, player.Center);

                for (int i = 0; i < 10; i++)
                {
                    Vector2 vel = (MathHelper.TwoPi * i / 10f).ToRotationVector2() * 6.5f;
                    Particle spark2 = new CustomSpark(player.Center + vel * 14, -vel * 0.1f, "CalamityMod/Particles/DemonSigilParticle", false, 22, 0.6f, (i % 2 == 0 ? Color.MediumOrchid : Color.BlueViolet) * 0.7f, new Vector2(1, 1), true, false, 0, false, false, -0.23f);
                    GeneralParticleHandler.SpawnParticle(spark2);

                    Dust c = Dust.NewDustPerfect(player.Center, ModContent.DustType<LightDust>());
                    c.velocity = vel;
                    c.scale = 1.7f;
                    c.noGravity = true;
                    c.color = (i % 2 != 0 ? Color.MediumOrchid : Color.BlueViolet);
                    c.noLightEmittence = true;
                }

                player.Calamity().demonSwordKillMode = true;

                int cooldownTime = KillMode.cooldownMax + KillMode.buffMax;
                player.Calamity().killModeCooldown = cooldownTime;
                player.AddCooldown(KillMode.ID, cooldownTime);
            }
            if (player.Calamity().demonSwordKillMode && player.ownedProjectileCounts[ModContent.ProjectileType<ForbiddenOathbladeHoldout>()] <= 0 && player.Calamity().killModeCooldown == KillMode.cooldownMax + KillMode.buffMax)
            {
                Projectile blade = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.MountedCenter,Vector2.Zero, ModContent.ProjectileType<ForbiddenOathbladeHoldout>(), Item.damage * 12, Item.knockBack, player.whoAmI, 0, throwCount);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            throwCount++;
            int useSpeed = (int)MathHelper.Clamp((Item.useTime / 2.8f), 1, 100);
            Projectile blade = Projectile.NewProjectileDirect(source, player.MountedCenter, velocity, type, damage, knockback, player.whoAmI, 0, throwCount);
            blade.localAI[2] = useSpeed;
            blade.timeLeft += useSpeed;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BladecrestOathsword>().
                AddIngredient<OldLordClaymore>().
                AddIngredient<UnholyCore>(5).
                AddIngredient<EssenceofHavoc>(8).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
