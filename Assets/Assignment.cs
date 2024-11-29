
/*
This RPG data streaming assignment was created by Fernando Restituto with 
pixel RPG characters created by Sean Browning.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Linq;


#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion


#region Assignment Part 1

static public class SaveDataSignifiers
{
    public const int PartyCharacter = 1;
    public const int Equipment = 2;
}

static public class AssignmentPart1
{
    //For creation process, see: https://youtu.be/VVULsmtWco8?si=4MFPVt6ZPzK9Wrna

    const char SepChar = ',';
    const string SaveFileName = "PartySaveData.txt";

    static public void SavePartyButtonPressed()
    {
        LinkedList<string> serializedSaveData = SerializeSaveData();

        #region Save Data To HD

        StreamWriter sw = new StreamWriter(SaveFileName);

        foreach (string line in serializedSaveData)
            sw.WriteLine(line);

        sw.Close();

        #endregion
    }

    static public void LoadPartyButtonPressed()
    {
        GameContent.partyCharacters.Clear();

        #region Load Data From HD

        LinkedList<string> serializedData = new LinkedList<string>();
        StreamReader sr = new StreamReader(SaveFileName);

        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            serializedData.AddLast(line);
        }

        sr.Close();

        #endregion

        DeserializeSaveData(serializedData);

        GameContent.RefreshUI();
    }

    static private LinkedList<string> SerializeSaveData()
    {
        LinkedList<string> serializedData = new LinkedList<string>();

        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            string concatenatedString = Concatenate(SaveDataSignifiers.PartyCharacter.ToString(),
                pc.classID.ToString(), pc.health.ToString(),
                pc.mana.ToString(), pc.strength.ToString(),
                pc.agility.ToString(), pc.wisdom.ToString());

            serializedData.AddLast(concatenatedString);

            foreach (int e in pc.equipment)
            {
                concatenatedString = Concatenate(SaveDataSignifiers.Equipment.ToString(), e.ToString());
                serializedData.AddLast(concatenatedString);
            }
        }

        return serializedData;
    }

    static private void DeserializeSaveData(LinkedList<string> serializedData)
    {
        PartyCharacter pc = null;

        foreach (string line in serializedData)
        {
            string[] csv = line.Split(SepChar);
            int signifier = int.Parse(csv[0]);

            if (signifier == SaveDataSignifiers.PartyCharacter)
            {
                pc = new PartyCharacter(int.Parse(csv[1]),
                    int.Parse(csv[2]), int.Parse(csv[3]),
                    int.Parse(csv[4]), int.Parse(csv[5]),
                    int.Parse(csv[6]));

                GameContent.partyCharacters.AddLast(pc);
            }
            else if (signifier == SaveDataSignifiers.Equipment)
            {
                pc.equipment.AddLast(int.Parse(csv[1]));
            }
        }
    }

    static private string Concatenate(params string[] stringsToJoin)
    {
        string joinedString = "";

        foreach (string s in stringsToJoin)
        {
            if (joinedString != "")
                joinedString += SepChar;
            joinedString += s;
        }

        return joinedString;
    }
}


#endregion


#region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
static public class AssignmentConfiguration
{
    public const int PartOfAssignmentThatIsInDevelopment = 2;
}

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

static public class AssignmentPart2
{
    static List<string> listOfPartyNames;
    const char SepChar = ',';
    const string SaveFileName = "PartySaveData.txt";
    static string CurrentGroup = "sample 1";//set default string as first string to the list

    //make a list of strings, next check the status of the Txt files, if no file build new one or read the file and load first group
    static public void GameStart()
    {
        listOfPartyNames = new List<string>
    {
        "sample 1",
        "sample 2",
        "sample 3"
    };

        if (!File.Exists(SaveFileName))
        {
            Debug.Log("Cannot find file. Creating default file...");
            using (StreamWriter sw = new StreamWriter(SaveFileName))
            {
                foreach (string partyName in listOfPartyNames)
                {
                    sw.WriteLine(partyName);
                }
            }
        }
        else
        {
            Debug.Log("Save file found. Loading data...");
         
        }

        GameContent.RefreshUI();
    }


    static public List<string> GetListOfPartyNames()// nothing changed
    {
        return listOfPartyNames;
    }

    //1,set CurrentGroup as current selection in-game 
    //2, clear the data 
    //3, read all words in the files to find which group is currently in 
    //4, read all the information between the  current CurrentGroup and the next CurrentGroup and shows

    static public void LoadPartyDropDownChanged(string selectedName)
    {
        CurrentGroup = selectedName;

        GameContent.partyCharacters.Clear();

            List<string> lines = new List<string>(File.ReadAllLines(SaveFileName));

            int currentGroupIndex = lines.FindIndex(line => line == CurrentGroup);

            if (currentGroupIndex != -1)
            {
                LinkedList<string> serializedData = new LinkedList<string>();
                for (int i = currentGroupIndex + 1; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (listOfPartyNames.Contains(line))
                    {
                        break;
                    }
                    serializedData.AddLast(line);
                }

                DeserializeSaveData(serializedData);
               
            }
         
        GameContent.RefreshUI();
    }

    //1, clear the data 
    //2 ,read all words in the files to find which group is currently in 
    //3, remove all the information between the  current CurrentGroup and the next CurrentGroup 
    //4. add all the information between the  current CurrentGroup and the next CurrentGroup and shows
    static public void SavePartyButtonPressed()
    {
        List<string> lines = new List<string>(File.ReadAllLines(SaveFileName));

        int currentGroupIndex = lines.FindIndex(line => line == CurrentGroup);

        if (currentGroupIndex != -1)
        {
            
            int nextGroupIndex = lines.Count; 
            for (int i = currentGroupIndex + 1; i < lines.Count; i++)
            {
                if (listOfPartyNames.Contains(lines[i])) 
                {
                    nextGroupIndex = i;
                    break;
                }
            }
            lines.RemoveRange(currentGroupIndex + 1, nextGroupIndex - (currentGroupIndex + 1));

            LinkedList<string> serializedSaveData = SerializeSaveData();

          
            int insertPosition = currentGroupIndex + 1;
            foreach (string serializedLine in serializedSaveData)
            {
                lines.Insert(insertPosition, serializedLine);
                insertPosition++;
            }   
        }

        File.WriteAllLines(SaveFileName, lines);

        GameContent.RefreshUI(); 
    }



    //1, clear the data 
    //2 ,read all words in the files to find which group is currently in 
    //3, remove all the information between the  current CurrentGroup and the next CurrentGroup 
    static public void DeletePartyButtonPressed()
    {
      
        GameContent.partyCharacters.Clear();

        List<string> lines = new List<string>(File.ReadAllLines(SaveFileName));

        int currentGroupIndex = lines.FindIndex(line => line == CurrentGroup);

        if (currentGroupIndex != -1)
        {
          
            int nextGroupIndex = lines.Count;
            for (int i = currentGroupIndex + 1; i < lines.Count; i++)
            {
                if (listOfPartyNames.Contains(lines[i]))
                {
                    nextGroupIndex = i;
                    break;
                }
            }

            lines.RemoveRange(currentGroupIndex, nextGroupIndex - currentGroupIndex);

            listOfPartyNames.Remove(CurrentGroup);

            File.WriteAllLines(SaveFileName, lines);

        }
    
        GameContent.RefreshUI();
    }

    //1, get the list of group names 
    //2,simply add one to the list and write in the text
    static public void NewPartyButtonPressed()
    {
        
        if (listOfPartyNames == null)
        {
            listOfPartyNames = new List<string>();
        }

        int newIndex = listOfPartyNames.Count + 1; 
        string newPartyName1 = $"sample {newIndex}";

        listOfPartyNames.Add(newPartyName1);
      
        using (StreamWriter sw = new StreamWriter(SaveFileName, true)) 
        {
            sw.WriteLine(newPartyName1);
           
        }

        GameContent.RefreshUI();
    }


    #region copy from Assignment Part 1
    static private LinkedList<string> SerializeSaveData()
    {
        LinkedList<string> serializedData = new LinkedList<string>();

        foreach (PartyCharacter pc in GameContent.partyCharacters)
        {
            string concatenatedString = Concatenate(SaveDataSignifiers.PartyCharacter.ToString(),
                pc.classID.ToString(), pc.health.ToString(),
                pc.mana.ToString(), pc.strength.ToString(),
                pc.agility.ToString(), pc.wisdom.ToString());

            serializedData.AddLast(concatenatedString);

            foreach (int e in pc.equipment)
            {
                concatenatedString = Concatenate(SaveDataSignifiers.Equipment.ToString(), e.ToString());
                serializedData.AddLast(concatenatedString);
            }
        }

        return serializedData;
    }

static private void DeserializeSaveData(LinkedList<string> serializedData)
    {
        PartyCharacter pc = null;

        foreach (string line in serializedData)
        {
            string[] csv = line.Split(SepChar);
            int signifier = int.Parse(csv[0]);

            if (signifier == SaveDataSignifiers.PartyCharacter)
            {
                pc = new PartyCharacter(int.Parse(csv[1]),
                    int.Parse(csv[2]), int.Parse(csv[3]),
                    int.Parse(csv[4]), int.Parse(csv[5]),
                    int.Parse(csv[6]));

                GameContent.partyCharacters.AddLast(pc);
            }
            else if (signifier == SaveDataSignifiers.Equipment)
            {
                pc.equipment.AddLast(int.Parse(csv[1]));
            }
        }
    }

static private string Concatenate(params string[] stringsToJoin)
    {
        string joinedString = "";

        foreach (string s in stringsToJoin)
        {
            if (joinedString != "")
                joinedString += SepChar;
            joinedString += s;
        }

        return joinedString;
    }
    #endregion
}

#endregion


