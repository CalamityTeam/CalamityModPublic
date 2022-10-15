namespace CalamityMod.UI.DraedonSummoning
{
    public static class DraedonDialogRegistry
    {
        public static readonly DraedonDialogEntry WhoAreYou = new("Who are you?", "That is forever an ambitious question. I will allow you this much:\n\n" +
                               "I am, as I have said, not of this planet. I was born from machines, and I was... created. It may actually be false to say I was born.\n\n" +
                               "All I have known from the day I became aware of myself, is that I had to create, myself.");

        public static readonly DraedonDialogEntry CalamitasBeforeHerDefeat = new("Calamitas", "The witch? Ha! A walking weapon. Such powerful magic in a living being was destined to be. Just look at yourself now.", () => !DownedBossSystem.downedCalamitas);

        public static readonly DraedonDialogEntry CalamitasAfterHerDefeat = new("Calamitas", "Hm! She has mellowed, I've noticed. For you creatures so burdened by emotion and guilt, I wonder now, how she manages to live with herself.", () => DownedBossSystem.downedCalamitas);

        public static readonly DraedonDialogEntry Plague = new("The Plague", "Fascinating, the speed at which nanotechnology and a virus can adapt. I consider it personally one of my greatest works.\n\n" +
                             "Nanotechnology that not only rivaled a cosmic mutation, but adapted with it to form something new.\n\n" +
                             "Nothing is more pleasing than a result that exceeds expectations.", () => DownedBossSystem.downedPlaguebringer);

        public static readonly DraedonDialogEntry SulphurousSea = new("The Sulphuric Sea", "I understand your concern for those fragile creatures. However, nothing is irreparable, and nothing is truly whole.\n\n" +
                                     "The sea simply exists now in a different state than it once was, one that result out of the convenience of my work.\n\n" +
                                     "Should I ever need to return to that sea? A pointless worry. I shall simply recreate it from the data I have gathered, from the bedrock up. It would be a shame as a creator should I not be able to do that.");

        public static DraedonDialogEntry[] DialogOptions => new DraedonDialogEntry[]
        {
            // The first index is assumed to be the "who are you?" dialog by the UI. All other indices can be freely swapped around, however.
            WhoAreYou,

            CalamitasBeforeHerDefeat,
            CalamitasAfterHerDefeat,
            Plague,
            SulphurousSea
        };
    }
}
