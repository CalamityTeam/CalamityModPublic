using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using static CalamityMod.CalamityUtils;
using Terraria.DataStructures;
using System.Linq;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("WulfrumBlade")]
    public class WulfrumScrewdriver : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static int DefaultTime = 10;
        public static readonly SoundStyle ThrustSound = new("CalamityMod/Sounds/Item/WulfrumScrewdriverThrust") { PitchVariance = 0.4f };
        public static readonly SoundStyle ThudSound = new("CalamityMod/Sounds/Item/WulfrumScrewdriverThud") { PitchVariance = 0.2f, Volume = 0.7f };
        public static readonly SoundStyle ScrewGetSound = new("CalamityMod/Sounds/Item/WulfrumScrewdriverScrewGet") { PitchVariance = 0.1f};
        public static readonly SoundStyle ScrewHitSound = new("CalamityMod/Sounds/Item/WulfrumScrewdriverScrewHit") { Volume = 0.7f };
        public static readonly SoundStyle FunnyUltrablingSound = new("CalamityMod/Sounds/Custom/UltrablingHit");

        public static bool ScrewQeuedForStorage = false;
        public bool ScrewStored = false;
        public bool ScrewAvailable => ScrewStored && ScrewTimer == 0;
        public static Vector3 ScrewStart = new Vector3(0);
        public static Vector3 ScrewPosition;
        public static Vector2 PrevOffset;
        public static float ScrewTimer;
        public static float ScrewTime = 40;
        public static Asset<Texture2D> ScrewTex;
        public static Asset<Texture2D> ScrewOutlineTex;
        public static float ScrewBaseDamageMult = 1.5f;
        public static float ScrewBazingaModeDamageMult = 6.5f;
        public static float ScrewBazingaAimAssistAngle = 0.52f; //This may look high but remember this is the FULL angle, so it actually checks for half that angle deviation
        public static float ScrewBazingaAimAssistReach = 600f;

        public override ModItem Clone(Item item)
        {
            return base.Clone(item);

            ModItem clone = base.Clone(item);
            if (clone is WulfrumScrewdriver a && item.ModItem is WulfrumScrewdriver a2 && a2.ScrewStored)
            {
                a.ScrewStored = a2.ScrewStored;
            }
            return clone;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            //Super speedy
            if (player.altFunctionUse == 2)
                return 2f;

            return base.UseSpeedMultiplier(player);
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 50;
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = DefaultTime + WulfrumScrewdriverProj.MaxTime;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = DefaultTime + WulfrumScrewdriverProj.MaxTime;
            Item.useTurn = true;
            Item.knockBack = 3.75f;
            Item.UseSound = ThrustSound;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<WulfrumScrewdriverProj>();
            Item.shootSpeed = 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumMetalScrap>(12).
                AddIngredient<EnergyCore>().
                AddTile(TileID.Anvils).
                Register();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) => ScrewStored = false;
        public override void UpdateInventory(Player player)
        {
            if (player.ActiveItem() != Item)
                ScrewStored = false;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;

            if (Main.myPlayer == player.whoAmI)
            {
                if (ScrewQeuedForStorage)
                {
                    ScrewStored = true;
                    ScrewQeuedForStorage = false;
                }

                if (ScrewTimer > 0)
                    ScrewTimer--;

                if (ScrewTimer == 1)
                {
                    Vector2 dustPos = new Vector2(ScrewPosition.X, ScrewPosition.Y) + Main.screenPosition;
                    int numDust = Main.rand.Next(5, 15);
                    for (int i = 0; i < numDust; i++)
                    {
                        Dust.NewDustPerfect(dustPos, Main.rand.NextBool() ? 246 : 247, Main.rand.NextVector2Circular(1f, 1f), Scale: Main.rand.NextFloat(0.9f, 1.4f));
                    }

                    SoundEngine.PlaySound(ScrewGetSound);
                }
            }

            base.HoldItem(player);
        }

        public override bool AltFunctionUse(Player player) => ScrewAvailable;
        public override bool CanUseItem(Player player) => player.altFunctionUse == 2 ? !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<WulfrumScrew>()) : true;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        { 
            if (player.altFunctionUse == 2) damage = (int)(damage * ScrewBaseDamageMult);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Vector2 chuckSpeed = new Vector2(Math.Sign(velocity.X) * 0.4f, -1.24f) + player.velocity / 4f;

                //Prevent it from being thrown at any less than -1f
                chuckSpeed.Y = Math.Clamp(chuckSpeed.Y, -1f, 3f);

                int p = Projectile.NewProjectile(source, position, chuckSpeed, ModContent.ProjectileType<WulfrumScrew>(), damage, knockback, player.whoAmI);

                ScrewStored = false;
                return false;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public CurveSegment InitialAway = new CurveSegment(SineOutEasing, 0f, 0f, -0.2f, 3);
        public CurveSegment AccelerateTowards = new CurveSegment(PolyInEasing, 0.3f, -0.2f, 1.2f, 3);
        public CurveSegment Bump1Segment = new CurveSegment(SineBumpEasing, 0.5f, 1f, 0.24f);
        public CurveSegment Bump2Segment = new CurveSegment(SineBumpEasing, 0.8f, 1f, -0.1f);
        internal float ProgressionOfScrew => PiecewiseAnimation(ScrewTimer / ScrewTime, new CurveSegment[] { InitialAway, AccelerateTowards, Bump1Segment, Bump2Segment });

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!ScrewStored)
                return;

            Player myPlayer = Main.LocalPlayer;

            if (myPlayer.ActiveItem() != Item || !myPlayer.active || myPlayer.dead)
                return;

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            if (ScrewTex == null)
                ScrewTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/WulfrumScrew");
            if (ScrewOutlineTex == null)
                ScrewOutlineTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/WulfrumScrewOutline");

            Texture2D screwTex = ScrewTex.Value;
            Texture2D screwOutlineTex = ScrewOutlineTex.Value;

            Vector2 realIdealSpot = myPlayer.MountedCenter + myPlayer.gfxOffY * Vector2.UnitY - Main.screenPosition - Vector2.UnitY * 50f - Vector2.Lerp(myPlayer.velocity, PrevOffset, 0.5f);
            realIdealSpot.Y += (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 5f;
            realIdealSpot.X += (float)Math.Sin(Main.GlobalTimeWrappedHourly * 1f) * 7.8f;
            
            ScrewPosition = new Vector3(realIdealSpot, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f) * MathHelper.PiOver4 * 0.34f);

            position = Vector2.Lerp(new Vector2(ScrewPosition.X, ScrewPosition.Y), new Vector2(ScrewStart.X, ScrewStart.Y), ProgressionOfScrew);
            float rotation = ScrewPosition.Z.AngleLerp(ScrewStart.Z, ProgressionOfScrew);
            float outlineOpacity = (float)Math.Pow(1 - ScrewTimer / ScrewTime, 2);
            scale = 1.05f + 0.05f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f);

            Main.spriteBatch.Draw(screwOutlineTex, position, null, Color.Lerp(Color.GreenYellow, Color.White, (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.5f + 0.5f) * outlineOpacity, rotation, screwOutlineTex.Size() / 2f, scale, 0, 0);
            Main.spriteBatch.Draw(screwTex, position, null, Color.White, rotation, screwTex.Size() / 2f, scale, 0, 0);

            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);


            PrevOffset = myPlayer.velocity;
            base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}
