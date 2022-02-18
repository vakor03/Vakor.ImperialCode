using System;
using System.IO;

string inputPath = "../../../input.txt";
string outputPath = "../../../output2.txt";

const int linesPerPage = 45;

string[] words = Array.Empty<string>();
int[][] wordPages = Array.Empty<int[]>();
int[] repeatedWordsCount = Array.Empty<int>();

int currentPage = 0;
int strCount = 0;
int wordsCount = 0;

int i = 0;

StreamReader streamReader = new StreamReader(inputPath);
readFile:
{
    if (streamReader.EndOfStream)
    {
        goto endReading;
    }

    String str = streamReader.ReadLine();

    if (strCount % linesPerPage == 0)
    {
        currentPage++;
    }

    strCount++;

    int j = 0;

    string word = String.Empty;
    checkString:
    {
        if (j == str.Length)
        {
            goto endCheckingString;
        }

        char symbol = str[j];

        if (symbol is >= 'A' and <= 'Z')
        {
            word += (char) (symbol + 32);

            if (j + 1 < str.Length)
            {
                goto endCheckingString;
            }
        }
        else if (symbol is >= 'a' and <= 'z' || word != "" && symbol is '-' or '\'')
        {
            word += symbol;

            if (j + 1 < str.Length)
            {
                goto endCheckingString;
            }
        }

        if (word != "")
        {
            i = 0;
            checkWordInArray:
            {
                if (i == wordsCount)
                {
                    goto addNewWord;
                }

                if (word == words[i])
                {
                    word = "";
                    if (repeatedWordsCount[i] > 100)
                    {
                        goto endCheckingString;
                    }

                    repeatedWordsCount[i]++;
                    if (repeatedWordsCount[i] <= wordPages[i].Length)
                    {
                        wordPages[i][repeatedWordsCount[i] - 1] = currentPage;
                    }
                    else
                    {
                        int[] pagesTmp = new int[repeatedWordsCount[i] * 2];
                        int p = 0;
                        copyPages:
                        {
                            pagesTmp[p] = wordPages[i][p];
                            p++;
                            if (p < repeatedWordsCount[i] - 1)
                            {
                                goto copyPages;
                            }
                        }
                        wordPages[i] = pagesTmp;
                        wordPages[i][repeatedWordsCount[i] - 1] = currentPage;
                    }

                    goto endCheckingString;
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
                int[][] expandedPagesArray = new int[words.Length == 0 ? 1 : words.Length * 2][];
                int[] expandedRepeatedArray = new int[words.Length == 0 ? 1 : words.Length * 2];

                i = 0;
                copyOldArray:
                {
                    if (i == words.Length)
                    {
                        words = expandedWordsArray;
                        repeatedWordsCount = expandedRepeatedArray;
                        wordPages = expandedPagesArray;

                        goto addWordToArray;
                    }

                    expandedWordsArray[i] = words[i];
                    expandedPagesArray[i] = wordPages[i];
                    expandedRepeatedArray[i] = repeatedWordsCount[i];

                    i++;
                    goto copyOldArray;
                }
            }


            addWordToArray:
            {
                words[wordsCount] = word;
                repeatedWordsCount[wordsCount] = 1;
                wordPages[wordsCount] = new[] {currentPage};
                word = String.Empty;
                wordsCount++;
            }
        }
    }
    endCheckingString:
    {
        j++;
        if (j < str.Length)
        {
            goto checkString;
        }
    }


    goto readFile;
}

endReading:
{
    streamReader.Close();
}

i = 1;
sort:
{
    int currentElement = repeatedWordsCount[i];
    string word = words[i];
    int[] currPages = wordPages[i];
    var k = i - 1;
    checkPreviousElements:
    {
        if (k >= 0)
        {
            int symbol = 0;
            compareWords:
            {
                if (symbol == words[k].Length || words[k][symbol] < word[symbol])
                {
                    goto addLastElement;
                }

                if (symbol + 1 < word.Length && words[k][symbol] == word[symbol])
                {
                    symbol++;
                    goto compareWords;
                }
            }

            repeatedWordsCount[k + 1] = repeatedWordsCount[k];
            words[k + 1] = words[k];
            wordPages[k + 1] = wordPages[k];
            k--;
            goto checkPreviousElements;
        }
    }
    addLastElement:
    {
        repeatedWordsCount[k + 1] = currentElement;
        words[k + 1] = word;
        wordPages[k + 1] = currPages;
        i++;
    }

    if (i < wordsCount)
    {
        goto sort;
    }
}

StreamWriter streamWriter = new StreamWriter(outputPath);
i = 0;
writeToFile:
{
    if (repeatedWordsCount[i] <= 100)
    {
        streamWriter.Write(words[i] + " - " + wordPages[i][0]);
        int j = 1;
        writePages:
        {
            if (j == repeatedWordsCount[i])
            {
                goto endWritingPages;
            }

            if (wordPages[i][j] != wordPages[i][j - 1])
            {
                streamWriter.Write(", " + wordPages[i][j]);
            }

            j++;
            goto writePages;
        }

        endWritingPages:
        {
            streamWriter.WriteLine();
        }
    }

    i++;
    if (i < wordsCount)
    {
        goto writeToFile;
    }
}

streamWriter.Close();