using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("DraedonsExoblade")]
    public class Exoblade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle SwingSound = new("CalamityMod/Sounds/Item/ExobladeSwing") { MaxInstances = 3, PitchVariance = 0.6f, Volume = 0.8f };
        public static readonly SoundStyle BigSwingSound = new("CalamityMod/Sounds/Item/ExobladeBigSwing") { MaxInstances = 3, PitchVariance = 0.2f };
        public static readonly SoundStyle BigHitSound = new("CalamityMod/Sounds/Item/ExobladeBigHit") { PitchVariance = 0.2f };
        public static readonly SoundStyle BeamHitSound = new("CalamityMod/Sounds/Item/ExobladeBeamSlash") { Volume = 0.4f, PitchVariance = 0.2f };
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Item/ExobladeDash") { Volume = 0.6f };
        public static readonly SoundStyle DashHitSound = new("CalamityMod/Sounds/Item/ExobladeDashImpact") { Volume = 0.85f };


        public static int BeamNoHomeTime = 24;

        public static float NotTrueMeleeDamagePenalty = 0.46f;

        public static float ExplosionDamageFactor = 1.8f;

        public static float LungeDamageFactor = 1.75f;

        public static int LungeCooldown = 60 * 3; //Projectile has 3 updates : aka 1 second

        public static float LungeMaxCorrection = MathHelper.PiOver4 * 0.05f;

        public static float LungeSpeed = 60f;

        public static float ReboundSpeed = 6f;

        public static float PercentageOfAnimationSpentLunging = 0.6f;

        public static int OpportunityForBigSlash = 37 * 3;

        public static float BigSlashUpscaleFactor = 1.5f;

        public static int DashTime = 49;

        public static int BaseUseTime = 49;
        public static int BeamsPerSwing = 3;

        public override void SetDefaults()
        {
            Item.width = 138;
            Item.height = 184;
            Item.damage = 915;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = BaseUseTime;
            Item.useAnimation = BaseUseTime;
            Item.useTurn = true;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.knockBack = 9f;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.shoot = ProjectileType<ExobladeProj>();
            Item.shootSpeed = 9f;
            Item.rare = RarityType<Violet>();
        }

        public override bool CanShoot(Player player)
        {
            //Lunge can't be used if ANY exoblade is there (even the ones in stasis)
            if (player.altFunctionUse == 2)
                return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ExobladeProj>());


            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ExobladeProj>() &&         
            !(n.ai[0] == 1 && n.ai[1] == 1)); //Ignores exoblades in post bonk stasis.
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;
            player.Calamity().mouseWorldListener = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? CanHitNPC(Player player, NPC target) => false;

        public override bool CanHitPvp(Player player, Player target) => false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float state = 0;

            //If there are any exoblades in "stasis" after a bonk, the attack should be an empowered slash instead
            if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ExobladeProj>() && n.ai[0] == 1 && n.ai[1] == 1 && n.timeLeft > LungeCooldown))
            {
                state = 2;

                //Put all the "post bonk" stasised exoblades into regular cooldown for the right click ljunge
                for (int i = 0; i < Main.maxProjectiles; ++i)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active || p.owner != player.whoAmI || p.type != Item.shoot || p.ai[0] != 1 || p.ai[1] != 1)
                        continue;

                    p.timeLeft = LungeCooldown;
                    p.netUpdate = true;
                    p.netSpam = 0;
                }
            }

            if (player.altFunctionUse == 2)
            {
                state = 1;
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, state, 0);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Terratomere>().
                AddIngredient<EntropicClaymore>().
                AddIngredient<AnarchyBlade>().
                AddIngredient<FlarefrostBlade>().
                AddIngredient<MiracleMatter>().
                AddTile(TileType<DraedonsForge>()).
                Register();
        }
    }
}
