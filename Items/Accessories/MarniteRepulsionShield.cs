using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    [LegacyName("MarniteBayonet")]
    public class MarniteRepulsionShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Marnite Repulsion Shield");
            Tooltip.SetDefault("Enemies behind you are struck by high-knockback hardlight quills\n" +
                "[c/FFF191:Backstabbers Beware!]");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MarniteRepulsionShieldPlayer modPlayer = player.GetModPlayer<MarniteRepulsionShieldPlayer>();
            modPlayer.shieldEquipped = true;

            if (player.whoAmI == Main.myPlayer)
            {
                int baseDamage = 5;
                var source = player.GetSource_Accessory(Item);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MarniteRepulsionHitbox>()] < 1)
                {
                    var hitbox = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<MarniteRepulsionHitbox>(), baseDamage, 14f, Main.myPlayer);
                    hitbox.originalDamage = baseDamage;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AnyGoldBar", 5).
                AddIngredient(ItemID.Granite, 15).
                AddIngredient(ItemID.Marble, 15).
                AddTile(TileID.Anvils).
                Register();

        }
    }

    public class MarniteRepulsionShieldPlayer : ModPlayer
    {
        public bool shieldEquipped = false;

        public override void ResetEffects()
        {
            shieldEquipped = false;
        }

        public override void UpdateDead()
        {
            shieldEquipped = false;
        }
    }

    public class MarniteRepulsionHitbox : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public Player Owner => Main.player[Projectile.owner];

        public static Asset<Texture2D> QuillTex;
        public List<QuillVisualData> Quills;
        public ref float NewQuillCooldown => ref Projectile.localAI[0];
        public static int QuillTime = 50;
        public struct QuillVisualData
        {
            public float rotation;
            public int time;

            public QuillVisualData(float _rotation)
            {
                rotation = _rotation;
                time = QuillTime;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Repulsion Matrix");
        }


        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 700;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (Owner.active && Owner.GetModPlayer<MarniteRepulsionShieldPlayer>().shieldEquipped)
            {
                Projectile.Center = Owner.Center + Vector2.UnitX * Owner.direction * -20f;
                if (Owner.mount.Active)
                {
                    Projectile.Center += -Vector2.UnitY * Owner.mount.PlayerOffset;
                }
            }

            else
                Projectile.active = false;
        }

        public override bool? CanHitNPC(NPC target)
        { 
            //Only enemies that are behind the player (shouldn't happen but just in case
            if (Math.Sign((Owner.Center - target.Center).X) != Owner.direction)
                return false;

            return base.CanHitNPC(target);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = Math.Sign(-Owner.direction);
        }

        public override bool? CanCutTiles() => false;

        public CurveSegment burstOut = new CurveSegment(PolyInOutEasing, 0f, 0f, 1f, 3);
        public CurveSegment stay = new CurveSegment(LinearEasing, 0.25f, 1f, 0f);
        public CurveSegment retract = new CurveSegment(PolyOutEasing, 0.8f, 1f, -0.8f);
        internal float QuillDisplace(float quillProgress) => PiecewiseAnimation(quillProgress, new CurveSegment[] { burstOut, stay, retract });


        public override bool PreDraw(ref Color lightColor)
        {
            return false;

            if (QuillTex == null)
                QuillTex = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/MarniteRepulsionShield_Quill");
            Texture2D quillTex = QuillTex.Value;

            if (Quills == null)
                Quills = new List<QuillVisualData>();

            NewQuillCooldown--;
            if (NewQuillCooldown <= 0)
            {
                Quills.Add(new QuillVisualData(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)));
                NewQuillCooldown = QuillTime / 4f;
            }

            for (int i = 0; i < Quills.Count; i++)
            {
                QuillVisualData quill = Quills[i];

                float rotation = quill.rotation + (Owner.direction < 0 ? MathHelper.Pi : 0);

                float bounce = (float)Math.Sin((quill.time / (float)QuillTime) * MathHelper.Pi) * 2f;
                float quillOpacity = Math.Min(bounce, 1f);
                float quillDisplacement = QuillDisplace(1 - quill.time / (float)QuillTime) * 14f;

                Vector2 quillPosition = Projectile.Center + Vector2.UnitX * (Projectile.width / 2f - 10f) * Owner.direction;
                quillPosition += Vector2.UnitX.RotatedBy(rotation) * quillDisplacement;

                Vector2 quillOrigin = new Vector2(quillTex.Width / 2f, quillTex.Height);

                Main.EntitySpriteDraw(quillTex, quillPosition - Main.screenPosition, null, lightColor * quillOpacity, rotation + MathHelper.PiOver2 * Owner.direction, quillOrigin, 1f, SpriteEffects.None, 0);

                quill.time--;
                Quills[i] = quill;
            }

            Quills.RemoveAll(q => q.time < 0);

            return false;
        }
    }
}
