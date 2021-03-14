using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WpfApp.PlayfairCipher
{
  public partial class MainWindow : Window
  {
    string Key;
    const string EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    string PlayfairString;
    string[,] PlayfairSquare = new string[5, 5];
    int PairsLenght;
    string InputPairs;
    string InputText;
    string OutputText;
    public MainWindow()
    {
      InitializeComponent();
      MessageBox.Show("Используется английский алфавит. Неалфавитные символы (цифры, " +
        "пробелы, знаки препинания) и не английские буквы игнорируются.\n" +
        "1. Буква 'J' заменяется на 'I' чтобы сформировать квадрат 5x5.\n" +
        "2. 'X' используется как дополнительный символ, когда вам надо " +
        "дополнить биграмму или разделить две одинаковые буквы.\n" +
        "3. Квадрат Плейфера заполняется построчно, начиная с ключевого слова.");
    }
    // Browse button click event to open file
    private void buttonBrowse_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog()
      {
        CheckFileExists = false,
        CheckPathExists = true,
        Multiselect = false,
        Title = "Choose file",
        Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
      };

      if (dialog.ShowDialog() == true)
      {
        tbInput.Text = File.ReadAllText(dialog.FileName, Encoding.UTF8).ToUpper();
      }
    }
    // GetInputText
    private void GetInputText()
    {
      InputText = "";
      for (int i = 0; i < tbInput.Text.Length; i++)
      {
        if (EnglishAlphabet.Contains(tbInput.Text[i].ToString()))
        {
          InputText += tbInput.Text[i].ToString();
        }
      }
      //tbOutput.Text += "\r\n";
      //tbOutput.Text += "Text: " + InputText;
    }
    // GetPlayfairSquare
    private void GetPlayfairSquare()
    {
      PlayfairString = Key;
      for (int i = 0; i < EnglishAlphabet.Length; i++)
      {
        if (!PlayfairString.Contains(EnglishAlphabet[i].ToString()) && EnglishAlphabet[i].ToString() != "J")
        {
          PlayfairString += EnglishAlphabet[i];
        }
      }
      int k = 0;
      for (int i = 0; i < 5; i++)
      {
        for (int j = 0; j < 5; j++)
        {
          PlayfairSquare[i, j] = PlayfairString[k].ToString();
          k++;
        }
      }

      //tbOutput.Text += "\r\n";
      //tbOutput.Text += "Playfair Square: " + PlayfairString;
    }
    // GetKey
    private void GetKey()
    {
      Key = "";
      for (int i = 0; i < tbKey.Text.Length; i++)
      {
        if(EnglishAlphabet.Contains(tbKey.Text[i].ToString()) && !Key.Contains(tbKey.Text[i].ToString()))
        {
          if(tbKey.Text[i].ToString() == "J")
          {
            if(!Key.Contains("I")) Key += "I";
          }
          else
          {
            Key += tbKey.Text[i].ToString();
          }
        }
      }
      //tbOutput.Text = "Key: " + Key;
    }
    // GetInputPairs
    private void GetInputPairs()
    {
      InputPairs = "";
      PairsLenght = InputText.Length;
      for (int i = 0; i < InputText.Length - 1; i++)
      {
        if(InputText[i] == InputText[i+1])
        {
          InputPairs += $"{InputText[i]}X";
          PairsLenght++;
        }
        else
        {
          InputPairs += $"{InputText[i]}{InputText[i+1]}";
          i++;
        }
      }
      if (PairsLenght > InputPairs.Length) InputPairs += InputText[InputText.Length - 1];
      if (InputPairs.Length % 2 == 1)
      {
        InputPairs += "X";
      }
      //tbOutput.Text += "\r\n";
      //tbOutput.Text += "Pairs: " + InputPairs;
    }
    // Encrypt button click event
    private void buttonEncrypt_Click(object sender, RoutedEventArgs e)
    {
      tbOutput.Clear();
      GetKey();
      GetInputText();
      GetPlayfairSquare();
      Encryption();
    }
    // Encrypt function
    private void Encryption()
    {
      GetInputPairs();
      OutputText = "";
      int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

      for (int k = 0; k < InputPairs.Length; k += 2)
      {
        for (int i = 0; i < 5; i++)
        {
          for (int j = 0; j < 5; j++)
          {
            if (PlayfairSquare[i, j] == InputPairs[k].ToString())
            {
              x1 = i;
              y1 = j;
            }
            if (PlayfairSquare[i, j] == InputPairs[k+1].ToString())
            {
              x2 = i;
              y2 = j;
            }
          }
        }
        // both in row
        if (x1 == x2)
        {
          if (y1 == 4) y1 = 0;
          else y1++;
          if (y2 == 4) y2 = 0;
          else y2++;
        }
        // both in column
        if (y1 == y2)
        {
          if (x1 == 4) x1 = 0;
          else x1++;
          if (x2 == 4) x2 = 0;
          else x2++;
        }
        // other cases
        if ((x1 != x2) && (y1 != y2))
        {
          int a = y2;
          y2 = y1;
          y1 = a;
        }
        
          OutputText += $"{PlayfairSquare[x1, y1]}{PlayfairSquare[x2, y2]}";
        
        
      }
      //tbOutput.Text += "\r\n";
      //tbOutput.Text += "Coded text: " + OutputText;
      tbOutput.Text += OutputText;
    }
    // Decrypt button click event
    private void buttonDecrypt_Click(object sender, RoutedEventArgs e)
    {
      tbOutput.Clear();
      GetKey();
      GetInputText();
      GetPlayfairSquare();
      Decryption();
    }
    // Decrypt function
    private void Decryption()
    {
      GetInputPairs();
      OutputText = "";
      int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

      for (int k = 0; k < InputPairs.Length; k += 2)
      {
        for (int i = 0; i < 5; i++)
        {
          for (int j = 0; j < 5; j++)
          {
            if (PlayfairSquare[i, j] == InputPairs[k].ToString())
            {
              x1 = i;
              y1 = j;
            }
            if (PlayfairSquare[i, j] == InputPairs[k + 1].ToString())
            {
              x2 = i;
              y2 = j;
            }
          }
        }
        // both in row
        if (x1 == x2)
        {
          if (y1 == 0) y1 = 4;
          else y1--;
          if (y2 == 0) y2 = 4;
          else y2--;
        }
        // both in column
        if (y1 == y2)
        {
          if (x1 == 0) x1 = 4;
          else x1--;
          if (x2 == 0) x2 = 4;
          else x2--;
        }
        // other cases
        if ((x1 != x2) && (y1 != y2))
        {
          int a = y2;
          y2 = y1;
          y1 = a;
        }
        OutputText += $"{PlayfairSquare[x1, y1]}{PlayfairSquare[x2, y2]}";
      }
      tbOutput.Text = OutputText;
    }
    // Browse button click event to save file
    private void buttonSave_Click(object sender, RoutedEventArgs e)
    {
      SaveFileDialog dialog = new SaveFileDialog()
      {
        Title = "Save file",
        Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
      };
      if (dialog.ShowDialog() == true)
      {
        File.WriteAllText(dialog.FileName, tbOutput.Text, Encoding.UTF8);
      }
    }
  }
}
