using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    //Dedicated to Dzicozan
    [AutoloadEquip(EquipType.Back)]
    public class TheCamper : ModItem
    {
        int auraCounter = 0;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("The Camper");
            Tooltip.SetDefault("In rest may we find victory.\n" +
                "You deal 90% less damage unless stationary\n" +
                "Standing still grants buff(s) dependent on what weapon you're holding\n" +
                "Standing still provides a damaging aura around you\n" +
                "While moving, you regenerate health as if standing still\n" +
                "Provides a small amount of light in the Abyss");
        }

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
            var source = player.GetProjectileSource_Accessory(Item);
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.camper = true;
            player.AddBuff(BuffID.HeartLamp, 60, true);
            player.AddBuff(BuffID.Campfire, 60, true);
            player.AddBuff(BuffID.WellFed, 60, true);
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
                            if (npc.active && !npc.friendly && npc.damage > -1 && !npc.dontTakeDamage && Vector2.Distance(player.Center, npc.Center) <= range)
                            {
                                Projectile p = Projectile.NewProjectileDirect(source, npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), (int)(Main.rand.Next(20, 41) * player.AverageDamage()), 0f, player.whoAmI, i);
                                if (!npc.buffImmune[BuffID.OnFire])
                                {
                                    npc.AddBuff(BuffID.OnFire, 120);
                                }
                            }
                        }
                    }
                    if (player.ActiveItem() != null && !player.ActiveItem().IsAir && player.ActiveItem().stack > 0)
                    {
                        bool summon = player.ActiveItem().CountsAsClass<SummonDamageClass>();
                        bool rogue = player.ActiveItem().Calamity().rogue;
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
                            modPlayer.throwingVelocity += 0.1f;
                        }
                        else if (melee)
                        {
                            player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
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
