using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class DragoonDrizzlefish : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Fishing";

        public bool ballShot = true;
        public int shotCounter = 0;
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true; //so it doesn't look weird af when holding it
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 42;
            Item.height = 38;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.1f;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DrizzlefishFireball>();
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Gel; // for GFB
        }
        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[GFB]", this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));
        public override bool CanUseItem(Player player)
        {
            if (!Main.zenithWorld)
            {
                return true;
            }
            if (player.altFunctionUse == 2)
            {
                if (Main.zenithWorld)
                {
                    CalamityGlobalItem.HasEnoughAmmo(player, Item, 5);
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(7, 7);

        public override bool AltFunctionUse(Player player) => Main.zenithWorld ? true : false;
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (Main.zenithWorld && player.altFunctionUse == 2 && player.Calamity().dragoonDrizzlefishGelBoost < 6)
                return true;
            else
                return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.zenithWorld && player.altFunctionUse == 2)
            {
                //GFB stuff
                if (player.Calamity().dragoonDrizzlefishGelBoost < 6)
                {
                    player.Calamity().dragoonDrizzlefishGelBoost++;
                    SoundStyle roar = Sunskater.DeathSound;
                    SoundEngine.PlaySound(roar with { Pitch = 0.3f }, player.Center);
                    SoundEngine.PlaySound(SoundID.Item2 with { Pitch = 0.3f }, player.Center);
                    CombatText.NewText(player.Hitbox, Color.OrangeRed, ":)");
                    for (int i = 0; i <= 11; i++)
                    {
                        Vector2 hVelocity = Main.rand.NextVector2Unit();
                        hVelocity.X *= 0.66f;
                        hVelocity *= Main.rand.NextFloat(1f, 2f);

                        int heart = Gore.NewGore(player.GetSource_FromThis(), player.Center + Main.rand.NextVector2Circular(14, 14), hVelocity, 331, Main.rand.NextFloat(0.2f, 1.3f));
                        Main.gore[heart].sticky = false;
                        Main.gore[heart].velocity *= 0.4f;
                        Main.gore[heart].velocity.Y -= 0.7f;
                    }
                }
            }
            else
            {
                if (Main.zenithWorld)
                {
                    if (Main.rand.NextBool(25) && player.Calamity().dragoonDrizzlefishGelBoost > 1)
                    {
                        SoundStyle roar = Sunskater.DeathSound;
                        SoundEngine.PlaySound(roar, player.Center);
                        player.Calamity().dragoonDrizzlefishGelBoost--;
                    }
                    if (player.Calamity().dragoonDrizzlefishGelBoost == 1 && Main.rand.NextBool(3))
                    {
                        bool MADFISH = Main.rand.NextBool(4) ? true : false;
                        CombatText.NewText(player.Hitbox, Color.OrangeRed, MADFISH ? ">:(" : ":(");
                        SoundStyle roar = Sunskater.DeathSound;
                        SoundEngine.PlaySound(roar with { Pitch = -0.3f }, player.Center);
                        if (MADFISH)
                            player.AddBuff(ModContent.BuffType<Dragonfire>(), 240, true);
                        return false;
                    }
                }
                
                velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5.5f));
                int shotType = ModContent.ProjectileType<DrizzlefishFireball>();
                if (shotCounter < 3)
                {
                    shotType = ModContent.ProjectileType<DrizzlefishFireball>();
                    shotCounter++;
                }
                else
                {
                    shotType = ModContent.ProjectileType<DrizzlefishFire>();
                    shotCounter = 0;
                }
                Projectile.NewProjectile(source, position, velocity, shotType, damage, knockback, player.whoAmI, 0f, Main.rand.Next(2));
            }
            
            return false;
        }
    }
}
