using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public static class TileEntityTimeHandler
    {
        public static void Update()
        {
            // Specifically on multiplayer clients, manually update the time variables of Power Cell Factories every frame.
            // This makes sure they animate. It will NOT produce cells; that code can only run server side.
            // Time is manually synced from the server every time a cell is created, so even under heavy lag they cannot stay desynced indefinitely.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                byte factoryType = ModContent.GetInstance<TEPowerCellFactory>().type;
                var enumerator = TileEntity.ByID.Values.GetEnumerator();
                do
                {
                    TileEntity te = enumerator.Current;
                    if (te != null && te.type == factoryType)
                    {
                        TEPowerCellFactory factory = (TEPowerCellFactory)te;
                        ++factory.Time;
                    }
                } while (enumerator.MoveNext());
            }
        }
    }
}
