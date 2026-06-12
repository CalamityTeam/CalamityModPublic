using System.Collections.Generic;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PolarisParrotfish : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public int ShotNumber = 0;
        public bool Happy = false;
        public float fireSpeed = 1;

        public static readonly SoundStyle Shot = new("CalamityMod/Sounds/Item/PolarisShot") { Volume = 0.6f };
        public static readonly SoundStyle Squeak = new("CalamityMod/Sounds/Custom/CuteSqueak") { Volume = 0.75f };

        public static int ArmorPenetration = 15;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
            Item.staff[Type] = true; //so it doesn't look weird af when holding it
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.damage = 35;
            Item.ArmorPenetration = ArmorPenetration;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.5f;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PolarStar>();
            Item.shootSpeed = 10f;
        }
        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[GFB]", this.GetLocalization(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal").Format(ArmorPenetration));
        public override bool AltFunctionUse(Player player) => Main.zenithWorld ? true : false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.zenithWorld && player.altFunctionUse == 2)
            {
                //GFB stuff
                if (Happy && Main.rand.NextBool(50)) // If you pet her too much, you will regret it
                {
                    Happy = false;
                    player.itemTime = 200;
                    player.itemAnimation = 200;
                    player.SetScreenshake(26f);
                    player.AddBuff(BuffID.Obstructed, 600);
                    Main.NewText(CalamityUtils.GetTextValue("Misc.Polaris0"), 255, 0, 0);
                    SoundEngine.PlaySound(Squeak with { Pitch = -1f }, player.Center);
                    SoundStyle roar = new("CalamityMod/Sounds/Custom/CeaselessVoidDeathBuild");
                    SoundEngine.PlaySound(roar with { Pitch = 0.5f }, player.Center);
                    int theFuckening = ModContent.ProjectileType<AstralFlame>();
                    int projDamage = 500;
                    int totalProjectiles = 50;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 projVelocity = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + projVelocity * 2000, -projVelocity * 4, theFuckening, projDamage, 0f, Main.myPlayer);
                    }
                }
                else // Pet Polaris
                {
                    SoundEngine.PlaySound(Squeak, player.Center);
                    CombatText.NewText(player.Hitbox, Color.Violet, "^-^");
                    Happy = true;
                    switch (Main.rand.Next(1, 5 + 1))
                    {
                        case 5:
                            Main.NewText(CalamityUtils.GetTextValue("Misc.Polaris1"), 72, 209, 204);
                            break;
                        case 4:
                            Main.NewText(CalamityUtils.GetTextValue("Misc.Polaris2"), 72, 209, 204);
                            break;
                        case 3:
                            Main.NewText(CalamityUtils.GetTextValue("Misc.Polaris3"), 72, 209, 204);
                            break;
                        case 2:
                            Main.NewText(CalamityUtils.GetTextValue("Misc.Polaris4"), 72, 209, 204);
                            break;
                        default:
                            Main.NewText(CalamityUtils.GetTextValue("Misc.Polaris5"), 72, 209, 204);
                            break;
                    }

                    for (int i = 0; i <= 6; i++)
                    {
                        Vector2 hVelocity = new Vector2(0, -4).RotateRandom(0.45);
                        hVelocity.X *= 0.66f;
                        hVelocity *= Main.rand.NextFloat(1f, 2f);

                        int heart = Gore.NewGore(player.GetSource_FromThis(), player.Center + velocity * 4, hVelocity, 331, Main.rand.NextFloat(0.2f, 1.3f));
                        Main.gore[heart].sticky = false;
                        Main.gore[heart].velocity *= 0.4f;
                        Main.gore[heart].velocity.Y -= 0.85f;
                    }
                }
            }
            else
            {
                if (Main.zenithWorld) // 1% chance to get tired when firing a projectile
                {
                    if (Happy && Main.rand.NextBool(100))
                    {
                        CombatText.NewText(player.Hitbox, Color.Violet, ">~<");
                        Happy = false;
                        SoundEngine.PlaySound(Squeak with { Pitch = -0.6f }, player.Center);
                    }
                    else
                        SoundEngine.PlaySound(Shot, player.Center);
                }
                else
                    SoundEngine.PlaySound(Shot, player.Center);

                for (int i = 0; i < (Happy ? 3 : 1); i++)
                    Projectile.NewProjectile(source, position + velocity * 5f, velocity.RotatedByRandom(0.05f * (i != 0 ? 6 : 1)), type, damage, knockback, player.whoAmI, 0f, ShotNumber);

                if (ShotNumber >= 2) // Cycle the shot color
                    ShotNumber = 0;
                else
                    ShotNumber++;
            }
            return false;
        }
        public override float UseSpeedMultiplier(Player player)
        {
            NPC target = player.Center.ClosestNPCAt(400);
            fireSpeed = (target == null ? 1 : Utils.Remap(Utils.Distance(player.Center, target.Center), 100, 400, 2, 1, true));
            return (Happy ? fireSpeed * 2 : fireSpeed);
        }
    }
}
