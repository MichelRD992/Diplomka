using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LinearniRovnice
{
    public class Matice
    { 
        public float[,] nezname { get; set; }

        public float[] konstanty { get; set; }

        public void Vytvor(int n)
        {
            nezname = new float[n, n];
            konstanty = new float[n];
        }

        public float DET(float[,] a)
        {
            int n = a.GetLength(0);
            if (n == 2)
                return a[0, 0] * a[1, 1] - a[0, 1] * a[1, 0];
            else
            {
                float det = 0;
                for (int i = 0; i < n; i++)
                {
                    if (a[0, i] == 0)
                        continue;
                    if(i % 2 == 0)
                        det += a[0, i] * DET(SubMatice(i, a));
                    else
                        det -= a[0, i] * DET(SubMatice(i, a));
                }
                return det;
            }
        }

        private float[,] SubMatice(int y, float[,] a)
        {
            int n = a.GetLength(0);
            float[,] sub = new float[n - 1, n - 1];
            int indexX = 0;
            for (int i = 1; i < n; i++)
            {
                int indexY = 0;
                for (int j = 0; j < n; j++)
                    if (j != y)
                        sub[indexX, indexY++] = a[i, j];
                indexX++;
            }
            return sub;
        }

        public float[] Gauss()
        {
            int n = konstanty.Length;
            float[,] a = Eliminace(n);
            float[] vysledky = new float[n];

            for (int i = n - 1; i >= 0; i--)
            {
                float konst = a[i, n];
                for (int j = i + 1; j < n; j++)
                    konst -= a[i, j] * vysledky[j];
                vysledky[i] = konst / a[i, i];
            }
            return vysledky;
        }

        public float[] Jordan()
        {
            int n = konstanty.Length;
            float[,] a = Eliminace(n);
            float[] vysledky = new float[n];

            //jedničky na diagonále
            for (int i = 0; i < n; i++)
            {
                float koef = a[i, i];
                if (koef != 1)
                    for (int j = i; j <= n; j++)
                        a[i, j] = a[i, j] / koef;
            }

            //eliminace na jednotkovou matici + konstanty
            for (int j = n - 1; j > 0; j--)
            {
                for (int i = j - 1; i >= 0; i--)
                {
                    float y = a[i, j];
                    float x = a[i, n] - a[j, n] * a[i, j];
                    a[i, n] -= a[j, n] * a[i, j];
                    a[i, j] = 0; //nemá smysl dávat a[j, i] -= a[j, i] * a[i, i], protože a[i, i] je v tomto případě vždy 1
                }
                vysledky[j] = a[j, n];
            }

            vysledky[0] = a[0, n];
            return vysledky;
        }

        private float[,] Eliminace(int n)
        {
            float[,] a = new float[n, n + 1];

            for(int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    a[i, j] = nezname[i, j];
                a[i, n] = konstanty[i];
            }

            for (int i = 0; i < n - 1; i++)
            {
                if (a[i, i] == 0)
                    a = Pivotace(a, i, n);
                for (int j = i + 1; j < n; j++)
                {
                    float koefX = a[i, i];
                    float koefY = a[j, i];
                    for (int k = i; k <= n; k++)
                        a[j, k] = a[j, k] * koefX - a[i, k] * koefY;
                }
            }
            return a;
        }

        private float[,] Pivotace(float[,] matice, int i, int n)
        {
            for (int j = i + 1; j < n; j++)
                if (matice[j, i] != 0)
                {
                    for (int k = 0; k <= n; k++)
                    {
                        float zastupce = matice[i, k];
                        matice[i, k] = matice[j, k];
                        matice[j, k] = zastupce;
                    }
                    return matice;
                }
            return null;
        }

        public float[] Vypocet(float presnost, bool jacobi)
        {
            int n = konstanty.Length;
            if (!Razeni(n)) return null; //pokud se matici nepovedlo seřadit, končí a vrací null

            float[] kontrol, apx = Enumerable.Repeat(0.0f, n).ToArray();
            bool hotovo;
            for (int i = 0; i < 15; i++)
                (hotovo, apx) = Iterace(apx, n, presnost, jacobi);
            (hotovo, kontrol) = Iterace(apx, n, presnost, jacobi);
            for (int i = 0; i < n; i++)
                if (Math.Abs(kontrol[i] - apx[i]) > 1) return null;
            while (hotovo == false)
                (hotovo, apx) = Iterace(apx, n, presnost, jacobi);
            return apx;
        }

        private bool Razeni(int n)
        {
            //oveří, zda nejsou na hlavní diagonále nuly
            bool diagonala = true;
            for (int i = 0; i < n; i++)
                if (nezname[i, i] == 0)
                {
                    diagonala = false;
                    break;
                }
            if (diagonala) return true;

            //vrátí seznam všech možných umístění pro každý řádek
            Dictionary<int, List<int>> seznam = new Dictionary<int, List<int>>();
            for(int i = 0; i < n; i++)
            {
                List<int> radek = new List<int>();
                for (int j = 0; j < n; j++)
                    if (nezname[i, j] != 0)
                        radek.Add(j);
                seznam.Add(i, radek);
            }

            //vypočítá vhodné pořadí
            int[] poradi = new int[n];
            for(int i = 0; i < n; i++)
            {
                var radek = seznam.OrderBy(k => k.Value.Count).First();
                if (radek.Value.Count == 0)
                    return false;
                poradi[radek.Key] = radek.Value[0];
                seznam.Remove(radek.Key);
                foreach(var x in seznam)
                    seznam[x.Key].Remove(radek.Value[0]);
            }

            //seřadí matici
            float[,] matice = new float[n, n];
            float[] konst = new float[n];
            for(int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    matice[poradi[i], j] = nezname[i, j];
                konst[poradi[i]] = konstanty[i];
            }
            nezname = matice;
            konstanty = konst;
            return true;
        }

        private (bool, float[]) Iterace(float[] apx1, int n, float presnost, bool jacobi)
        {
            bool hotovo = true;
            float[] apx2 = new float[n];
            apx1.CopyTo(apx2, 0);
            for (int i = 0; i < n; i++)
            {
                if (jacobi) apx2[i] = Hodnota(apx1, i, n);
                else apx2[i] = Hodnota(apx2, i, n);
                if (Math.Abs(apx1[i] - apx2[i]) > presnost)
                    hotovo = false;
            }
            return (hotovo, apx2);
        }

        private float Hodnota(float[] promenne, int i, int n)
        {
            float konst = konstanty[i];
            for (int j = 0; j < n; j++)
                if(i != j)
                    konst -= promenne[j] * nezname[i, j];
            konst = konst / nezname[i, i];
            return konst;
        }
    }
}
