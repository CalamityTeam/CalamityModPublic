using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Armor.Demonshade
{
    [AutoloadEquip(EquipType.Head)]
    public class DemonshadeHelm : ModItem, IExtendedHat
    {
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/AbilitySounds/DemonshadeEnrage");

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Demonshade Helm");
            Tooltip.SetDefault("30% increased damage and 15% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 50;
            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DemonshadeBreastplate>() && legs.type == ModContent.ItemType<DemonshadeGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var hotkey = CalamityKeybinds.SetBonusHotKey.TooltipHotkeyString();
            player.setBonus = "100% increased minion damage and +10 max minions\n" +
                "All attacks inflict the demon flame debuff\n" +
                "Shadowbeams and demon scythes will fire down when you are hit\n" +
                "A friendly red devil follows you around\n" +
                "Press " + hotkey + " to enrage nearby enemies with a dark magic spell for 10 seconds\n" +
                "This makes them do 25% more damage but they also take 125% more damage";
            var modPlayer = player.Calamity();
            modPlayer.dsSetBonus = true;
            modPlayer.wearingRogueArmor = true;
            modPlayer.WearingPostMLSummonerSet = true;
            if (player.whoAmI == Main.myPlayer && !modPlayer.chibii)
            {
                modPlayer.redDevil = true;
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<DemonshadeSetDevilBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<DemonshadeSetDevilBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<DemonshadeRedDevil>()] < 1)
                {
                    var baseDamage = 10000;
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(10000);
                    var devil = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<DemonshadeRedDevil>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    devil.originalDamage = baseDamage;
                }
            }
            player.GetDamage<SummonDamageClass>() += 1f;
            player.maxMinions += 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.3f;
            player.GetCritChance<GenericDamageClass>() += 15;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShadowspecBar>(12).
                AddTile<DraedonsForge>().
                Register();
        }

        public string ExtensionTexture => "CalamityMod/Items/Armor/Demonshade/DemonshadeHelm_Extension";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -4f);
    }
}
