using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    //Dedicated to Dzicozan
    [AutoloadEquip(EquipType.Back)]
    public class TheCamper : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        int auraCounter = 0;

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
            Item.defense = 10;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var source = player.GetSource_Accessory(Item);
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.camper = true;
            player.AddBuff(BuffID.HeartLamp, 60);
            Main.SceneMetrics.HasHeartLantern = true;
            player.AddBuff(BuffID.Campfire, 60);
            Main.SceneMetrics.HasCampfire = true;

            // Only hand out the buff if the player is not already fully fed. This prevents the player from being robbed of food.
            if (!player.HasBuff(BuffID.WellFed3))
                player.AddBuff(BuffID.WellFed3, 80);
            else
            {
                // Prevent it from flickering
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    if (player.buffType[l] == BuffID.WellFed3 && player.buffTime[l] < 80)
                        player.buffTime[l] = 80;
                }
            }
            
            Lighting.AddLight(player.Center, 0.825f, 0.66f, 0f);
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.StandingStill())
                {
                    player.GetDamage<GenericDamageClass>() += 0.15f;
                    auraCounter++;
                    float range = 200f;
                    if (auraCounter == 9)
                    {
                        auraCounter = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.IsAnEnemy() && !npc.dontTakeDamage && Vector2.Distance(player.Center, npc.Center) <= range)
                            {
                                int campingFireDamage = (int)player.GetBestClassDamage().ApplyTo(Main.rand.Next(20, 41));
                                Projectile.NewProjectileDirect(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), campingFireDamage, 0f, player.whoAmI, i);
                            }
                        }
                    }
                    if (player.ActiveItem() != null && !player.ActiveItem().IsAir && player.ActiveItem().stack > 0)
                    {
                        bool summon = player.ActiveItem().CountsAsClass<SummonDamageClass>();
                        bool rogue = player.ActiveItem().CountsAsClass<ThrowingDamageClass>();
                        bool melee = player.ActiveItem().CountsAsClass<MeleeDamageClass>();
                        bool ranged = player.ActiveItem().CountsAsClass<RangedDamageClass>();
                        bool magic = player.ActiveItem().CountsAsClass<MagicDamageClass>();
                        if (summon)
                        {
                            player.GetKnockback<SummonDamageClass>() += 0.1f;
                            player.AddBuff(BuffID.Bewitched, 60, true);
                        }
                        else if (rogue)
                        {
                            modPlayer.rogueVelocity += 0.1f;
                        }
                        else if (melee)
                        {
                            player.GetAttackSpeed<MeleeDamageClass>() += 0.1f;
                            player.AddBuff(BuffID.Sharpened, 60, true);
                        }
                        else if (ranged)
                        {
                            player.AddBuff(BuffID.AmmoBox, 60, true);
                        }
                        else if (magic)
                        {
                            player.AddBuff(BuffID.Clairvoyance, 60, true);
                        }
                    }
                }
                else
                {
                    auraCounter = 0;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ShinyStone).
                AddIngredient(ItemID.Campfire, 10).
                AddIngredient(ItemID.HeartLantern, 5).
                AddIngredient(ItemID.SharpeningStation).
                AddIngredient(ItemID.CrystalBall).
                AddIngredient(ItemID.AmmoBox).
                AddIngredient(ItemID.BewitchingTable).
                AddRecipeGroup("AnyFood", 50).
                AddTile(TileID.TinkerersWorkbench).
                Register();

        }
    }
}
