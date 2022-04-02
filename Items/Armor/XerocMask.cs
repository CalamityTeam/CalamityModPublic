using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class XerocMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Empyrean Mask");
            Tooltip.SetDefault("Wrath of the cosmos\n" +
                "11% increased rogue damage and critical strike chance, 5% increased movement speed\n" +
                "Temporary immunity to lava");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 40, 0, 0);
            item.rare = ItemRarityID.Red;
            item.defense = 20; //71
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<XerocPlateMail>() && legs.type == ModContent.ItemType<XerocCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.armorEffectDrawOutlines = true;
            player.Calamity().meldTransformation = true;
            player.Calamity().meldTransformationForce = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.xerocSet = true;
            modPlayer.rogueStealthMax += 1.15f;
            player.setBonus = "9% increased rogue damage and velocity\n" +
                "Rogue projectiles have special effects on enemy hits\n" +
                "Imbued with cosmic wrath and rage when you are damaged\n" +
                "Rogue stealth builds while not attacking and slower while moving, up to a max of 115\n" +
                "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
            {
                player.AddBuff(BuffID.Wrath, 2);
                player.AddBuff(BuffID.Rage, 2);
            }
            modPlayer.throwingDamage += 0.09f;
            modPlayer.throwingVelocity += 0.09f;
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.11f;
            player.Calamity().throwingCrit += 11;
            player.moveSpeed += 0.05f;
            player.lavaMax += 240;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MeldiateBar>(), 12);
            recipe.AddIngredient(ItemID.LunarBar, 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }

    public class MeldTransformationHead : EquipTexture
    {
        public override bool DrawHead()
        {
            return false;
        }
    }

    public class MeldTransformationBody : EquipTexture
    {
        public override bool DrawBody()
        {
            return false;
        }
    }

    public class MeldTransformationLegs : EquipTexture
    {
        public override bool DrawLegs()
        {
            return false;
        }
    }
}
