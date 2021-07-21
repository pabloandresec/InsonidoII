using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public static class Utils
{
    public static string[] alphabet = new string[]
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
        };
    public static Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0,1),
        Vector2Int.one,
        Vector2Int.right,
        new Vector2Int(1,-1),
        Vector2Int.down,
        new Vector2Int(-1,-1),
        Vector2Int.left,
        new Vector2Int(-1,1)
    };

    public static Vector2Int[] cardDirections = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public static string RandomString()
    {
        string rs = "";
        for (int i = 0; i < 10; i++)
        {
            rs += alphabet[Random.Range(0, alphabet.Length)];
        }
        return rs;
    }

    ///////////////////////Methods///////////////////////////////

    /// <summary>
    /// Converte un rango de valores a otro rango\n
    /// Ejemplo: Un valor (5) de un rango entre 0 y 10 sera convertido a (50) si el nuevo rango es 0 y 100
    /// </summary>
    public static int ConvertRangedValueToAnotherRange(int value, IntRange oldRange, IntRange newRange)
    {
        int oR = (oldRange.max - oldRange.min);
        int nR = (newRange.max - newRange.min);
        return (((value - oldRange.min) * nR) / oR) + newRange.min;
    }
    /// <summary>
    /// Converte un rango de valores a otro rango\n
    /// Ejemplo: Un valor (5) de un rango entre 0 y 10 sera convertido a (50) si el nuevo rango es 0 y 100
    /// </summary>
    public static float ConvertRangedValueToAnotherRange(float value, FloatRange oldRange, FloatRange newRange)
    {
        float oR = (oldRange.max - oldRange.min);
        float nR = (newRange.max - newRange.min);
        return (((value - oldRange.min) * nR) / oR) + newRange.min;
    }

    /// <summary>
    /// Regresa una letra desde un array que las contiene a partir de un indice
    /// </summary>
    /// <param name="i">Indice</param>
    /// <returns></returns>
    public static string GetLetterByIndex(int i)
    {
        if (i < alphabet.Length && i >= 0)
        {
            return alphabet[i];
        }
        else
        {
            return "*";
        }
    }


    /// <summary>
    /// Regresa un letra al azar en string
    /// </summary>
    /// <returns>letra al azar</returns>
    public static string RandomLetter()
    {
        int x = Random.Range(0, alphabet.Length);
        return alphabet[x];
    }

    public static Vector2Int GetLetterVectorIndex(int index, int faceSize)
    {
        return new Vector2Int(index / faceSize, index % faceSize);
    }
    public static int GetLetterIntIndex(Vector2Int index, int faceSize)
    {
        return faceSize * index.y + index.x;
    }

    /// <summary>
    /// Invierte el orden del string
    /// </summary>
    /// <returns>string invertida</returns>
    public static string ReverseString(string markedWord)
    {
        char[] c = markedWord.ToCharArray();
        Array.Reverse(c);
        return new string(c);
    }

    /// <summary>
    /// convierte un float de tiempo a un string de hora formateado
    /// </summary>
    /// <returns>string invertida</returns>
    public static string FloatTimeToFormattedString(float t)
    {
        Debug.Log("Converting " + t + " to formated string");
        TimeSpan ts = TimeSpan.FromSeconds(t);
        return string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
    }
    public static float ReverseNumber(float num, FloatRange range)
    {
        return (range.max + range.min) - num;
    }
    public static int ReverseNumber(int num, IntRange range)
    {
        return (range.max + range.min) - num;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void SetCameraInMiddleOfGrid(Renderer tileRenderer, Camera cam, Vector2Int gridSize, float gridBorder)
    {
        Vector3 tileSize = tileRenderer.bounds.size;
        Vector3 camPos = new Vector3(((float)gridSize.x / 2f) - (tileSize.x / 2), ((float)gridSize.y / 2f) - (tileSize.y / 2), -10);
        cam.transform.position = camPos;

        float aR = (float)Screen.width / (float)Screen.height;
        float verSize = (float)gridSize.y / 2f + gridBorder;
        float horSize = ((float)gridSize.x / 2f + gridBorder) / aR;
        cam.orthographicSize = (verSize > horSize) ? verSize : horSize;
    }

    public static void SetCameraInMiddleOfGrid(Vector3 pieceSize, Camera cam, Vector2Int gridSize, float gridBorder)
    {
        Debug.Log("Centering camera");
        Vector3 camPos = new Vector3(((float)gridSize.x / 2f) - (pieceSize.x / 2), ((float)gridSize.y / 2f) - (pieceSize.y / 2), -10);
        cam.transform.position = camPos;

        float aR = (float)Screen.width / (float)Screen.height;
        float verSize = (float)gridSize.y / 2f + gridBorder;
        float horSize = ((float)gridSize.x / 2f + gridBorder) / aR;
        cam.orthographicSize = (verSize > horSize) ? verSize : horSize;
    }

    public static bool IsInBounds(Vector2Int pos, Vector2Int gridSize)
    {
        return pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;
    }
}


[Serializable]
public struct Word
{
    public int face;
    public string text;
    public Vector2Int index;
    public Vector2Int dir;

    public string Serialize()
    {
        return text + "/" + index.x + "," + index.y + "/" + dir.x + "," + dir.y + "/" + face;
    }

    public static Word DeserializeAWord(string serializedWord)
    {
        Word t;
        string[] splits = serializedWord.Split('/');
        t.text = splits[0];

        string[] stringIndex = splits[1].Split(',');
        t.index = new Vector2Int(int.Parse(stringIndex[0]), int.Parse(stringIndex[1]));

        string[] stringDir = splits[2].Split(',');
        t.dir = new Vector2Int(int.Parse(stringDir[0]), int.Parse(stringDir[1]));

        string face = splits[3];
        t.face = int.Parse(face);

        return t;
    }

    public static string[] DeserializeData(string data)
    {
        string[] wordData = data.Split('|');
        if (wordData.Length > 1)
        {
            return wordData;
        }
        return null;
    }


    public static Word[] GetWordsFromArray(string[] parsedData)
    {
        List<Word> tWords = new List<Word>();
        for (int i = 1; i < parsedData.Length; i++)
        {
            Word w = DeserializeAWord(parsedData[i]);
            tWords.Add(w);
        }
        return tWords.ToArray();
    }
}

[Serializable]
public struct IntRange
{
    public int min;
    public int max;

    public IntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public IntRange GetPercentRange()
    {
        return new IntRange(0, 100);
    }
}