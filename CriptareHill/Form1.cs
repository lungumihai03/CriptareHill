using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CriptareHill
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int[][] GetKeyMatrix()
        {
            string[] rows = richTextBoxKey.Lines;
            if (rows.Length != 3)
            {
                throw new Exception("Matricea cheie trebuie să aibă exact 3 rânduri.");
            }
            int[][] key = new int[3][];
            for (int i = 0; i < 3; i++)
            {
                string[] elements = rows[i].Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (elements.Length != 3)
                {
                    throw new Exception($"Rândul {i + 1} din matricea cheie trebuie să conțină exact 3 elemente.");
                }
                key[i] = Array.ConvertAll(elements, int.Parse);
            }
            return key;
        }

        private int Determinant(int[][] key)
        {
            int det = key[0][0] * (key[1][1] * key[2][2] - key[1][2] * key[2][1])
                    - key[0][1] * (key[1][0] * key[2][2] - key[1][2] * key[2][0])
                    + key[0][2] * (key[1][0] * key[2][1] - key[1][1] * key[2][0]);
            return det;
        }

        private int ModInverse(int a, int m)
        {
            a = a % m;
            for (int x = 1; x < m; x++)
            {
                if ((a * x) % m == 1)
                {
                    return x;
                }
            }
            return 1;
        }

        private int GetCofactor(int[][] key, int row, int col)
        {
            int[][] minor = new int[2][];
            minor[0] = new int[2];
            minor[1] = new int[2];
            int m = 0, n = 0;
            for (int i = 0; i < 3; i++)
            {
                if (i == row) continue;
                n = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (j == col) continue;
                    minor[m][n] = key[i][j];
                    n++;
                }
                m++;
            }
            return (minor[0][0] * minor[1][1] - minor[0][1] * minor[1][0]);
        }

        private int[][] GetKeyInverse(int[][] key)
        {
            int[][] inverse = new int[3][];
            inverse[0] = new int[3];
            inverse[1] = new int[3];
            inverse[2] = new int[3];
            int det = Determinant(key);
            det = ((det % 26) + 26) % 26;
            int detInv = ModInverse(det, 26);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int sign = ((i + j) % 2 == 0) ? 1 : -1;
                    int cofactor = GetCofactor(key, i, j);
                    inverse[j][i] = ((sign * cofactor * detInv) % 26 + 26) % 26;
                }
            }
            return inverse;
        }

        private string Encrypt(string text, int[][] key)
        {
            string result = "";
            string processedText = "";

            // Pregătim fișierul cript.txt pentru a scrie detalii
            using (StreamWriter writer = new StreamWriter("cript.txt"))
            {
                foreach (char c in text)
                {
                    if (char.IsLetter(c))
                    {
                        processedText += char.ToUpper(c);
                    }
                }
                while (processedText.Length % 3 != 0)
                {
                    processedText += 'X';
                }

                writer.WriteLine("Text procesat pentru criptare: " + processedText);

                for (int i = 0; i < processedText.Length; i += 3)
                {
                    int p1 = processedText[i] - 'A';
                    int p2 = processedText[i + 1] - 'A';
                    int p3 = processedText[i + 2] - 'A';

                    // Calculăm valorile criptate
                    int c1 = (key[0][0] * p1 + key[0][1] * p2 + key[0][2] * p3) % 26;
                    int c2 = (key[1][0] * p1 + key[1][1] * p2 + key[1][2] * p3) % 26;
                    int c3 = (key[2][0] * p1 + key[2][1] * p2 + key[2][2] * p3) % 26;

                    // Scriem pașii detaliați în fișier
                    writer.WriteLine($"C1=({key[0][0]}*{p1}+{key[0][1]}*{p2}+{key[0][2]}*{p3}) mod 26 = " +
                                     $"({key[0][0] * p1}+{key[0][1] * p2}+{key[0][2] * p3}) mod 26 = {c1} ({(char)(c1 + 'A')})");
                    writer.WriteLine($"C2=({key[1][0]}*{p1}+{key[1][1]}*{p2}+{key[1][2]}*{p3}) mod 26 = " +
                                     $"({key[1][0] * p1}+{key[1][1] * p2}+{key[1][2] * p3}) mod 26 = {c2} ({(char)(c2 + 'A')})");
                    writer.WriteLine($"C3=({key[2][0]}*{p1}+{key[2][1]}*{p2}+{key[2][2]}*{p3}) mod 26 = " +
                                     $"({key[2][0] * p1}+{key[2][1] * p2}+{key[2][2] * p3}) mod 26 = {c3} ({(char)(c3 + 'A')})");

                    result += (char)(c1 + 'A');
                    result += (char)(c2 + 'A');
                    result += (char)(c3 + 'A');
                }
            }

            return result;
        }

        private string Decrypt(string text, int[][] key)
        {
            int[][] keyInverse = GetKeyInverse(key);
            if (keyInverse == null)
            {
                MessageBox.Show("Matricea cheie nu este inversabilă modulo 26.");
                return string.Empty;
            }

            string result = "";
            using (StreamWriter writer = new StreamWriter("decript.txt"))
            {
                writer.WriteLine("Text procesat pentru decriptare: " + text);

                for (int i = 0; i < text.Length; i += 3)
                {
                    int c1 = text[i] - 'A';
                    int c2 = text[i + 1] - 'A';
                    int c3 = text[i + 2] - 'A';

                    // Calculăm valorile decriptate
                    int p1 = (keyInverse[0][0] * c1 + keyInverse[0][1] * c2 + keyInverse[0][2] * c3) % 26;
                    int p2 = (keyInverse[1][0] * c1 + keyInverse[1][1] * c2 + keyInverse[1][2] * c3) % 26;
                    int p3 = (keyInverse[2][0] * c1 + keyInverse[2][1] * c2 + keyInverse[2][2] * c3) % 26;

                    // Ajustare pentru valori negative
                    p1 = (p1 + 26) % 26;
                    p2 = (p2 + 26) % 26;
                    p3 = (p3 + 26) % 26;

                    // Scriem pașii detaliați în fișier
                    writer.WriteLine($"D1=({keyInverse[0][0]}*{c1}+{keyInverse[0][1]}*{c2}+{keyInverse[0][2]}*{c3}) mod 26 = " +
                                     $"({keyInverse[0][0] * c1}+{keyInverse[0][1] * c2}+{keyInverse[0][2] * c3}) mod 26 = {p1} ({(char)(p1 + 'A')})");
                    writer.WriteLine($"D2=({keyInverse[1][0]}*{c1}+{keyInverse[1][1]}*{c2}+{keyInverse[1][2]}*{c3}) mod 26 = " +
                                     $"({keyInverse[1][0] * c1}+{keyInverse[1][1] * c2}+{keyInverse[1][2] * c3}) mod 26 = {p2} ({(char)(p2 + 'A')})");
                    writer.WriteLine($"D3=({keyInverse[2][0]}*{c1}+{keyInverse[2][1]}*{c2}+{keyInverse[2][2]}*{c3}) mod 26 = " +
                                     $"({keyInverse[2][0] * c1}+{keyInverse[2][1] * c2}+{keyInverse[2][2] * c3}) mod 26 = {p3} ({(char)(p3 + 'A')})");

                    result += (char)(p1 + 'A');
                    result += (char)(p2 + 'A');
                    result += (char)(p3 + 'A');
                }
            }

            return result;
        }


        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            label4.Text = "Rezultat Criptat: ";
            try
            {
                int[][] key = GetKeyMatrix();
                string inputText = textBoxInput.Text;
                string encryptedText = Encrypt(inputText, key);
                textBoxOutput.Text = encryptedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la criptare: " + ex.Message);
            }
        }

        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            int[][] key = GetKeyMatrix();
            label4.Text = "Rezultat Decriptat: ";
            try
            {
                string encryptedText = textBoxOutput.Text;
                string decryptedText = Decrypt(encryptedText, key);
                textBoxOutput.Text = decryptedText;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la decriptare: " + ex.Message);
            }
        }
    }
}
