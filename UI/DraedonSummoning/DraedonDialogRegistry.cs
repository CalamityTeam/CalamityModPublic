using System.Collections.Generic;

namespace CalamityMod.UI.DraedonSummoning
{
    public static class DraedonDialogRegistry
    {
        public static readonly DraedonDialogEntry WhoAreYou = new("Who are you?", "An ambitious question, if you are a philosopher. I will give only the facts.\n\n" +
            "I am not of this planet. I was born from machines. Or rather, I was... created. It may be misleading to say I was born.\n\n" +
            "The exact circumstances, I do not know. I do know that my first instance of awareness was that I was surrounded by a workshop.\n\n" +
            "All I have known since then was the act of creation and destruction.");

        public static readonly DraedonDialogEntry CalamitasBeforeHerDefeat = new("Calamitas", "The witch? She is a walking weapon. Such powerful magic in a living being was destined to be so. Look at yourself now.", () => !DownedBossSystem.downedSCal);

        public static readonly DraedonDialogEntry CalamitasAfterHerDefeat = new("Calamitas", "She has mellowed, I've noticed. For you creatures so burdened by emotion and guilt, I wonder now, how she manages to live with herself.", () => 
            DownedBossSystem.downedSCal);

        public static readonly DraedonDialogEntry Plague = new("The Plague", "Fascinating, isn't it? I wasn't able to control it, unfortunately, but for science there is no such thing as failure as long as you record the results.\n\n" +
            "I knew the Astral Infection was capable of converting even machinery, but for nanotechnology I'd given it to react in kind, and begin to use the virus as a building block itself...\n\n" +
            "Marvelous.", () => DownedBossSystem.downedPlaguebringer);
        
        public static readonly DraedonDialogEntry SulphurousSea = new("The Sulphuric Sea", "Your concern for the utter destruction of the ecosystem is understandable, for you biological, and sentimental folk. However, " +
            "nothing is irreparable or truly unrecreatable. Not even your sacred 'miracle of life'.\n\n" +
            "The sea simply exists now in a different state than what it once was, one that resulted out of convenience for my work.\n\n" +
            "Should I ever need to return to that sea? A pointless worry. I shall simply recreate it from the data I gathered, from the bedrock up. I would be a shame as a creator should I not be able to do that.");

        public static readonly DraedonDialogEntry TheTyrant = new("The Tyrant", "My patron...\n" +
            "He is not someone I have spoken to in a very long time.\n\n" +
            "Is that such a surprise? He no longer required my services and I no longer needed his patronage. Many years ago now he withdrew himself to some secret hideaway. I suspect, to the grave of dragons he spoke of...\n\n" +
            "Should you locate him, I'll await the news of either his or your death. Perhaps I will collect the cadaver.", () => DownedBossSystem.downedSCal);

        internal static List<DraedonDialogEntry> DialogOptions = new()
        {
            // The first index is assumed to be the "who are you?" dialog by the UI. All other indices can be freely swapped around, however.
            WhoAreYou,

            CalamitasBeforeHerDefeat,
            CalamitasAfterHerDefeat,
            Plague,
            SulphurousSea,
            TheTyrant
        };
    }
}
