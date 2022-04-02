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
            MultiplayerClientUpdateVisuals();
        }
        private static void MultiplayerClientUpdateVisuals()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;

            byte factoryType = ModContent.GetInstance<TEPowerCellFactory>().type;
            byte chargerType = ModContent.GetInstance<TEChargingStation>().type;
            byte codebreakerType = ModContent.GetInstance<TECodebreaker>().type;

            var enumerator = TileEntity.ByID.Values.GetEnumerator();
            do
            {
                TileEntity te = enumerator.Current;
                if (te == null)
                    continue;

                if (te.type == factoryType)
                {
                    // Specifically on multiplayer clients, manually update the time variables of Power Cell Factories every frame.
                    // This makes sure they animate. It will NOT produce cells; that code can only run server side.
                    // Time is manually synced from the server every time a cell is created, so even under heavy lag they cannot stay desynced indefinitely.
                    TEPowerCellFactory factory = (TEPowerCellFactory)te;
                    ++factory.Time;
                }
                else if (te.type == chargerType)
                {
                    // Specifically on multiplayer clients, produce charging dust when the "should dust" flag is set by the most recent sync packet.
                    // This makes sure they produce charging dust for all clients. It will NOT actually charge items; that code can only run server side.
                    TEChargingStation charger = (TEChargingStation)te;

                    if (charger.ClientChargingDust && charger.CanDoWork)
                    {
                        charger.ClientChargingDust = false;
                        charger.SpawnChargingDust();
                    }
                }
                else if (te is TEBaseTurret turret)
                {
                    // Perform any client-specific update tasks for this turret.
                    turret.UpdateClient();
                    
                    // Specifically on multiplayer clients, manually update the turret's rotation every frame. This is exactly the same code run server side.
                    // This makes sure they visually track targets in multiplayer. It will NOT fire projectiles; that code can only run server side.
                    turret.UpdateAngle();
                }
                else if (te.type == codebreakerType)
                {
                    // Specifically on multiplayer clients, manually update the time variables of the Codebreaker every frame.
                    // This is done so that clients can accurately gauge how complete decryptions are.
                    TECodebreaker codebreaker = (TECodebreaker)te;
                    codebreaker.UpdateTime();
                }
            } while (enumerator.MoveNext());
        }
    }
}
