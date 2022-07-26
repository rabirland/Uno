namespace Uno.Client;

public static class Emoji
{
    public static readonly string[] PlayerIconEmojis = GeneratePlayerEmojis();

    public static string GetPlayerEmoji(string playerName)
    {
        var index = playerName.Select(c => (int)c).Sum();
        int max = PlayerIconEmojis.Length;
        var normalizedIndex = index % max;

        return PlayerIconEmojis[normalizedIndex];
    }

    private static string[] GeneratePlayerEmojis()
    {
        const int person = 0x1F9D1; // The Torso-less basic "person" emoji
        const int man = 0x1F468; // The Torso-less basic "man" emoji
        const int woman = 0x1F469; // The Torso-less basic "woman" emoji
        const int male = 0x2642; // The "male" sign
        const int female = 0x2640; // The "female" sign

        List<string> htmlCodes = new List<string>();
        int[] personTypes = new int[] // Hex codes of emojis that can be combined with the male and female signs
        {
            // Gestures
            0x1F64D, // Frowing
            0x1F64E, // Pouting
            0x1F645, // Person No
            0x1F646, // Person OK
            0x1F481, // Person Tipping Hand
            0x1F64B, // Person Raising Hand
            0x1F647, // Person Bowing
            0x1F926, // Person Facepalming
            0x1F937, // Person Shrugging

            // Professions
            0x1F46E, // Police officer
            0x1F575, // Detective
            0x1F482, // Royal Guard
            0x1F477, // Construction worker
        };

        int[] basePersonCombinators = new int[] // Hex codes of emojis that are combinators for the person, male and female emojis.
        {
            0x2695, // Health Worker (Medical symbol)
            0x1F393, // Student (Hat)
            0x1F3EB, // Teacher (school)
            0x2696, // Judge (law mark)
            0x1F33E, // Farmer (crop)
            0x1F373, // Cook (frying pan)
            0x1F527, // Mechanic (wrench)
            0x1F3ED, // Factory worker (factory)
            0x1F4BC, // Office worker (suit case)
            0x1F52C, // Scientist (microscope)
            0x1F4BB, // Technologist (laptop)
            0x1F3A4, // Singer (mic)
            0x1F3A8, // Artist (paint pallette)
            0x2708, // Pilot (plane)
            0x1F680, // Astronaut (space ship)
            0x1F692, // Firefighter (fire truck)
        };

        htmlCodes.Add("&#x1F977;"); // Ninja, has no male/female variant
        htmlCodes.Add("&#x1FAC5;"); // King, has explicit variant for male / female
        htmlCodes.Add("&#x1F934;"); // Prince, has explicit variant for male / female
        htmlCodes.Add("&#x1F478;"); // Princess, has explicit variant for male / female

        // Add grestures
        foreach (var gestureCode in personTypes)
        {
            htmlCodes.Add($"&#x{gestureCode:X};"); // Person
            htmlCodes.Add($"&#x{gestureCode:X};&#x200D;&#x{male:X};&#xFE0F;"); // Man
            htmlCodes.Add($"&#x{gestureCode:X};&#x200D;&#x{female:X};&#xFE0F;"); // Woman
        }

        // Add professions
        foreach (var professionCode in basePersonCombinators)
        {
            htmlCodes.Add($"&#x{person:X};;&#x200D;&#x{professionCode:X};"); // Person
            htmlCodes.Add($"&#x{man:X};;&#x200D;&#x{professionCode:X};"); // Man
            htmlCodes.Add($"&#x{woman:X};;&#x200D;&#x{professionCode:X};"); // Woman
        }

        return htmlCodes.ToArray();
    }
}
