using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PayParking
{
    /*  Clasa Masina reprezinta entitatea care parcheaza, prin membrii numar de inmatriculare,
     *  orele de intrare si de iesire din parcare si suma totala pe care o are de platit la plecare,
     *  alaturi de setteri si getteri.
    */
    public class Masina
    {
        private string nrInmatriculare;
        public string NrInmatriculare
        {
            get { return nrInmatriculare; }
            set { nrInmatriculare = value; }
        }

        private DateTime oraIntrare;
        public DateTime OraIntrare
        {
            get { return oraIntrare; }
            set { oraIntrare = value; }
        }

        private DateTime oraIesire;
        public DateTime OraIesire
        {
            get { return oraIesire; }
            set { oraIesire = value; }
        }

        private int totalDePlata;
        public int GetTotalDePlata()
        {
            return totalDePlata;
        }
        /* Metoda set pentru totalul de plata calculeaza diferenta de timp dintre ora la care masina a intrat in parcare,
         * si ora la care aceasta iese din parcare, apoi folosind minutele totale din aceasta diferenta calculez pretul dupa
         * formula data.
         */
        public void SetTotalDePlata(int tarifPrimaOra, int tarifUlterior)
        {

            TimeSpan timpStationat = oraIesire.Subtract(oraIntrare);

            if (timpStationat.TotalMinutes <= 60)
            {
                totalDePlata = tarifPrimaOra;
            }
            else
            {
                double timpRotunjit = Math.Ceiling(timpStationat.TotalMinutes/60);
                totalDePlata = (int)(tarifPrimaOra + (timpRotunjit - 1) * tarifUlterior);
            }
        }

        public Masina()
        {
            nrInmatriculare = "";
        }

        public Masina(string value)
        {
            nrInmatriculare = value;
            oraIntrare = DateTime.Now;
        }

    }
    /* Clasa Parcare reprezinta entitatea parcarii reprezentata de membrii: lista masinilor parcate, care e o lista de perechi
     * (locul pe care masina este parcata si masina in sine), tarifele pentru prima ora respectiv orele ulterioare, 
     * numarul de locuri libere din parcare si numarul de bon care este camp static pentru a fi unic pentru clasa si a fi incrementat
     * fiecare obiect creat (masina care parcheaza) avand un numar diferit
     */
    public class Parcare
    {
        private List<(int index,Masina masina)> listaMasini;
        private int tarifPrimaOra = 10;
        private int tarifUlterior = 5;
        private int nrLocuriLibere;
        private static int nrBon = 0;

        public Parcare()
        {
            listaMasini = new List<(int index,Masina masina)>();
            nrLocuriLibere = 10;
        }
        /* Metoda Intrare este apelata atunci cand o masina doreste sa intre in parcare. Verific daca sunt locuri libere si daca 
         * locul dorit este liber, in caz contrar aruncand exceptii.
         */
        public void Intrare(string nrInmatriculare, int locAles)
        {
            if (nrLocuriLibere > 0)
            {
                (int index, Masina masina) car = listaMasini.Find(m => m.index == locAles);
                if (car.masina == null)
                {
                    listaMasini.Add((locAles, new Masina(nrInmatriculare)));
                    SetNrLocuriLibere(nrLocuriLibere - 1);
                }
                else
                {
                    throw new Exception("Locul este ocupat deja");
                }
            }
            else
            {
                throw new Exception("Parcarea este ocupata la capacitate maxima momentan");
            }
        }
        /* Metoda Iesire este apelata atunci cand o masina doreste sa paraseasca parcarea. Caut masina in lista dupa 
         * numarul de inmatriculare si verific daca exista, in caz contrar aruncand exceptie.
         */
        public void Iesire(string nrInmatriculare)
        {

            (int index,Masina masina) car = listaMasini.Find(m => m.masina.NrInmatriculare == nrInmatriculare);
            if (car.masina != null)
            {
                car.masina.OraIesire = DateTime.Now;
                /*pentru testarea functionalitatilor legate de pret sau de sumarul final 
                decomentati linia urmatoare si modificati numarul de minute dupa bunul plac*/
                //car.masina.OraIesire = DateTime.Now.AddMinutes(114);
                Console.WriteLine(car.masina.OraIesire);
                car.masina.SetTotalDePlata(tarifPrimaOra, tarifUlterior);
                nrBon++;

                GenereazaSumar(car.masina);

                listaMasini.Remove(car);
                SetNrLocuriLibere(nrLocuriLibere + 1);
            }
            else
            {
                throw new Exception("Aceasta masina nu exista in parcare. Introduceti numarul de inmatriculare corect");
            }
        }

        public int GetNrLocuriLibere()
        {
            return nrLocuriLibere;
        }

        private void SetNrLocuriLibere(int value)
        {
            nrLocuriLibere = value;
        }

        public List<(int,Masina)> GetListaMasini()
        {
            return listaMasini;
        }

        public int GetTarifPrimaOra()
        {
            return tarifPrimaOra;
        }

        public int GetTarifUlterior()
        {
            return tarifUlterior;
        }
        /* Metoda GenereazaSumar este apelata atunci cand o masina iese din parcare si genereaza un bon care contine cateva informatii
         * legate de sederea in parcare precum: numarul bonului si data la care a fost emis, numarul de inmatriculare al masinii,
         * orele de intrare/iesire, timpul pentrecut in parcare si suma de plata.
         */
        public void GenereazaSumar(Masina masina)
        {
            TimeSpan span = masina.OraIesire.Subtract(masina.OraIntrare);

            Console.WriteLine("\nNr. Bon: {0}/{1}",nrBon,DateTime.Now.ToString("dd/MM/yyyy"));
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Nr. Inmatriculare: {0}", masina.NrInmatriculare);
            Console.WriteLine("Ora check-in: {0}", masina.OraIntrare.ToString("hh:mm:ss tt"));
            Console.WriteLine("Ora check-out: {0}", masina.OraIesire.ToString("hh:mm:ss tt"));
            Console.WriteLine("Timp stationat: {0}", span.ToString(@"hh\:mm\:ss"));
            Console.WriteLine("--------------------------------");
            Console.WriteLine("TOTAL DE PLATA: {0} LEI", masina.GetTotalDePlata());
        }
    }

    class Program
    {
        /* Am ales sa fac un meniu in clasa main folosind un switch pentru a facilita utilizarea si testarea aplicatiei.
         * Tot aici se creaza instanta parcarii.
         */
        static void Main(string[] args)
        {
            Parcare parcare = new Parcare();
            bool running = true;
            // o expresie regulata pe care o folosesc pt a valida ca numarul de inmatriculare sa fie de forma "JD-NR-ABC"
            var validation = new Regex("^[A-Z][A-Z]-\\d\\d-[A-Z][A-Z][A-Z]$"); 

            while (running){
                Console.Clear();
                Console.WriteLine("1. Afisare numar locuri libere");
                Console.WriteLine("2. Intra in parcare");
                Console.WriteLine("3. Iesire din parcare");
                Console.WriteLine("4. Afisare lista masini parcate");
                Console.WriteLine("0. Exit\n");

                Console.WriteLine("Introduceti optiunea:");
                string menu = Console.ReadLine();
                Console.Clear();

                switch (menu)
                {
                    case "1":
                        Console.WriteLine("In acest moment in parcare sunt {0} locuri disponibile",parcare.GetNrLocuriLibere());
                        Console.WriteLine("\nApasati orice tasta pentru a va reintoarce la meniu");
                        Console.ReadKey();
                        break;

                    case "2":
                        /*in acest for este construit desenul pentru locurile de parcare. Pe randul 2 si 5 in fiecare loc de parcare 
                        este ori X daca locul e ocupat ori un numar reprezentand locul de parcare.*/
                        for(int i = 0; i < 30; i++)
                        {
                            if (i % 5 == 0 && i != 0) { Console.Write("|\n"); }
                            if (i == 15) { Console.WriteLine("-------------------------------"); }
                            if (i >= 5 && i <= 9)
                            {
                                (int index, Masina masina) car = parcare.GetListaMasini().Find(m => m.Item1 == i % 5 + 1);
                                if (car.masina == null)
                                {
                                    Console.Write("|  {0}  ", i % 5 + 1);
                                }
                                else
                                {
                                    Console.Write("|  X  ");
                                }
                            }else if(i>=20 && i <= 24)
                            {
                                (int index, Masina masina) car = parcare.GetListaMasini().Find(m => m.Item1 == i % 5 + 6);
                                if (car.masina == null)
                                {
                                    if (i % 5 + 6 == 10) { Console.Write("|  {0} ", i % 5 + 6); }
                                    else { Console.Write("|  {0}  ", i % 5 + 6); }
                                }
                                else
                                {
                                    Console.Write("|  X  ");
                                }
                            }
                            else
                            {
                                Console.Write("|     ");
                            }
                            if(i==29) { Console.WriteLine("|\n"); }
                        }

                        Console.WriteLine("X - loc ocupat\n");

                        Console.WriteLine("Pe ce loc doriti sa parcati ?");
                        string locAles = Console.ReadLine();
                        int value;

                        //verific daca locul de parcare tastat este in limite
                        while (!int.TryParse(locAles,out value) || int.Parse(locAles) < 1 || int.Parse(locAles) > 10)
                        {
                            Console.WriteLine("Loc incorect!");
                            Console.WriteLine("Introduceti un numar intre 1-10 inclusiv");
                            locAles = Console.ReadLine();
                        }

                        Console.WriteLine("Introduceti numarul de inmatriculare sub forma 'JJ-NR-ABC':");
                        string numarIntrare = Console.ReadLine();
                        
                        //validez daca nr de inmatriculare tastat este in format corect
                        while (!validation.IsMatch(numarIntrare))
                        {
                            Console.Clear();
                            Console.WriteLine("Numar incorect!");
                            Console.WriteLine("Introduceti numarul de inmatriculare sub forma 'JJ-NR-ABC':");
                            numarIntrare = Console.ReadLine();
                        }
                        
                        //incerc introducerea masinii in parcare, in caz contrar primesc o exceptie si afisez mesajul acesteia 
                        try 
                        { 
                            parcare.Intrare(numarIntrare,Int32.Parse(locAles)); 
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("\nApasati orice tasta pentru a va reintoarce la meniu");
                            Console.ReadKey();
                            break;
                        }

                        Console.WriteLine("\nBun venit! Tariful pentru prima ora este de {0} LEI," +
                            " iar pentru orice ora ulterioara este de {1} LEI/ora",parcare.GetTarifPrimaOra(),parcare.GetTarifUlterior());
                        Console.WriteLine("\nApasati orice tasta pentru a va reintoarce la meniu");
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.WriteLine("Introduceti numarul de inmatriculare sub forma 'JJ-NR-ABC':");
                        string numarIesire = Console.ReadLine();

                        while (!validation.IsMatch(numarIesire))
                        {
                            Console.Clear();
                            Console.WriteLine("Numar incorect!");
                            Console.WriteLine("Introduceti numarul de inmatriculare sub forma 'JJ-NR-ABC':");
                            numarIesire = Console.ReadLine();
                        }
                        try
                        {
                            parcare.Iesire(numarIesire);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                        Console.WriteLine("\nApasati orice tasta pentru a va reintoarce la meniu");
                        Console.ReadKey();
                        break;

                    case "4":
                        if (parcare.GetListaMasini().Count == 0)
                        {
                            Console.WriteLine("Nu sunt masini parcate in acest moment");
                            Console.WriteLine("\nApasati orice tasta pentru a va reintoarce la meniu");
                            Console.ReadKey();
                            break;
                        }

                        Console.WriteLine("Masinile parcate in acest moment sunt:");
                        foreach((int index,Masina masina) loc in parcare.GetListaMasini())
                        {
                            Console.WriteLine("{0} pe locul {1}",loc.masina.NrInmatriculare,loc.index);
                        }

                        Console.WriteLine("\nApasati orice tasta pentru a va reintoarce la meniu");
                        Console.ReadKey();
                        break;

                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Comanda invalida. Apasati orice tasta pentru a reveni la meniu");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
