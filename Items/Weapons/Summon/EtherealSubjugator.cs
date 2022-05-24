using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EtherealSubjugator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Subjugator");
            Tooltip.SetDefault("Summons a phantom to protect you\n" +
            "Each phantom takes only half of a minion slot");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 70;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(1, 40, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item82;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PhantomGuy>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Summon/EtherealSubjugatorGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                int i = Main.myPlayer;
                float num72 = Item.shootSpeed;
                player.itemTime = Item.useTime;
                Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
                float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
                float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (player.gravDir == -1f)
                {
                    num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector2.Y;
                }
                float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
                if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
                {
                    num78 = (float)player.direction;
                    num79 = 0f;
                    num80 = num72;
                }
                else
                {
                    num80 = num72 / num80;
                }
                num78 *= num80;
                num79 *= num80;
                vector2.X = (float)Main.mouseX + Main.screenPosition.X;
                vector2.Y = (float)Main.mouseY + Main.screenPosition.Y;
                Vector2 spinningpoint = new Vector2(num78, num79);
                spinningpoint = spinningpoint.RotatedBy(1.5707963705062866, default);
                int p = Projectile.NewProjectile(source, vector2.X + spinningpoint.X, vector2.Y + spinningpoint.Y, spinningpoint.X, spinningpoint.Y, type, damage, knockback, i, 0f, 1f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
