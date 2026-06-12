using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class PiggyGold : Piggy
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.GoldCrittersCollection.Add(Type);
            NPCID.Sets.NormalGoldCritterBestiaryPriority.Insert(NPCID.Sets.NormalGoldCritterBestiaryPriority.IndexOf(NPCID.GoldBunny) + 2, Type);
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.catchItem = (short)ModContent.ItemType<PiggyGoldItem>();
            Banner = 0;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // All gold critters have the same Bestiary entry.
            var flavorText = database.FindEntryByNPCID(NPCID.GoldBunny).Info.Where(info => info is FlavorTextBestiaryInfoElement).FirstOrDefault();
            bestiaryEntry.AddTags(
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface, 
                flavorText);
        }

        public override void AI()
        {
            base.AI();
            NPC.ProduceGoldCritterDust();
        }
    }
}
