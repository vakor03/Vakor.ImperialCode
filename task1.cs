using System;
using System.IO;


string[] stopWords = {"the", "in", "a", "an", "for", "to", "on"};

string pathToFile = "../../../input.txt";
string outputPath = "../../../output.txt";

string[] words = Array.Empty<string>();
int[] repeatedWordsCount = Array.Empty<int>();

int wordsCount = 0;

int maxWordsCount = 25;
string word = "";

StreamReader streamReader = new(pathToFile);

readFile:
{
    if (streamReader.EndOfStream)
    {
        goto finishReading;
    }

    char symbol = (char) streamReader.Read();

    if (symbol is >= 'A' and <= 'Z')
    {
        word += (char) (symbol + 32);

        if (!streamReader.EndOfStream)
        {
            goto readFile;
        }
    }
    else if (symbol is >= 'a' and <= 'z' || word != "" && symbol is '-' or '\'')
    {
        word += symbol;

        if (!streamReader.EndOfStream)
        {
            goto readFile;
        }
    }

    if (word != "")
    {
        int i = 0;

        checkForStopWord:
        {
            if (i == stopWords.Length)
            {
                i = 0;
                goto checkWordInArray;
            }

            if (stopWords[i] == word)
            {
                word = String.Empty;
                goto readFile;
            }

            i++;
            goto checkForStopWord;
        }


        checkWordInArray:
        {
            if (i == wordsCount)
            {
                goto addNewWord;
            }

            if (words[i] == word)
            {
                repeatedWordsCount[i]++;
                word = String.Empty;
                goto readFile;
            }

            i++;
            goto checkWordInArray;
        }

        addNewWord:
        {
            if (wordsCount != words.Length)
            {
                goto addWordToArray;
            }
        }


        // expanding array
        {
            string[] expandedWordsArray = new string[words.Length == 0 ? 1 : words.Length * 2];
            int[] expandedCountArray = new int[words.Length == 0 ? 1 : words.Length * 2];

            i = 0;
            copyOldArray:
            {
                if (i == words.Length)
                {
                    words = expandedWordsArray;
                    repeatedWordsCount = expandedCountArray;
                    goto addWordToArray;
                }

                expandedWordsArray[i] = words[i];
                expandedCountArray[i] = repeatedWordsCount[i];

                i++;
                goto copyOldArray;
            }
        }


        addWordToArray:
        {
            words[wordsCount] = word;
            repeatedWordsCount[wordsCount] = 1;
            word = String.Empty;
            wordsCount++;
        }
    }

    goto readFile;
}


finishReading:
{
    streamReader.Close();
}


int j = 1;
sortArray:
{
    int currentEl = repeatedWordsCount[j];
    word = words[j];

    int k = j - 1;
    checkPreviousElements:
    {
        if (k >= 0 && repeatedWordsCount[k] < currentEl)
        {
            repeatedWordsCount[k + 1] = repeatedWordsCount[k];
            words[k + 1] = words[k];
            k--;
            goto checkPreviousElements;
        }
    }

    repeatedWordsCount[k + 1] = currentEl;
    words[k + 1] = word;
    j++;

    if (j < wordsCount)
    {
        goto sortArray;
    }
}


StreamWriter streamWriter = new StreamWriter(outputPath);
j = 0;

writeWordToFile:
{
    streamWriter.WriteLine(words[j] + " - " + repeatedWordsCount[j]);
    j++;

    if (j < maxWordsCount && j < wordsCount)
    {
        goto writeWordToFile;
    }
}

streamWriter.Close();