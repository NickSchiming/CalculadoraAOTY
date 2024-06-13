using System.Text.RegularExpressions;

namespace CalculadoraAOTY
{
    internal abstract partial class Program
    {
        private static readonly char[] Separator = ['\r', '\n'];

        private static void Main()
        {
            var texto = File.ReadAllText("ratings.txt");
            var crindex = texto.IndexOf('\u00a9');
            string[] skipstring = ["Tracks", "feat.", "with", "\u00a9", "SAVE", "Updated"];

            if (crindex != -1)
            {
                var ratings = texto[crindex..];
                var lines = ratings.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                var scorepond = 0.0;
                var tempopond = 0.0;
                var totsecs = 0.0;
                var sumScore = 0.0;

                var index = 1;
                var cleanTimeStamp = "";
                foreach (var line in lines)
                {
                    
                    var skip = false;
                    var trim = line.Trim();
                    


                    if (line.Length > 3)
                    {
                        //Console.WriteLine(line);
                        if (skipstring.Any(s => line.Contains(s)))
                        {
                            skip = true;
                        }

                        if (!skip)
                        {
                            cleanTimeStamp = MyRegex().Replace(trim[^5..], "");
                            
                            var seconds = cleanTimeStamp[^2..];
                            var minutes = cleanTimeStamp[..^2].Replace(":", "");
                            totsecs = int.Parse(seconds) + int.Parse(minutes) * 60;
                            tempopond += totsecs;
                            

                        }
                    }
                    else if (MyRegex1().IsMatch(trim))
                    {
                        var score = int.Parse(trim);
                        sumScore += score;
                        scorepond += score * totsecs;
                        Console.WriteLine($"{index} - tempo: {cleanTimeStamp} - score: {score}");
                        index++;
                    }


                    
                }

                if (tempopond != 0)
                {
                    Console.WriteLine($"--------------------------");
                    var pondAverage = Math.Round(scorepond / tempopond);
                    var average = Math.Round(sumScore / index);
                    Console.WriteLine($"Média: {average}");
                    Console.WriteLine($"Média Ponderada: {pondAverage}");
                }
                else
                {
                    Console.WriteLine("Erro: A Soma dos tempos da smúsicas é igual a 0!");
                }



            }
            else
            {
                Console.WriteLine("Erro: Caractere de copyright não encontrado no texto!");
            }


            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        [GeneratedRegex(@"^[^\d]*")]
        private static partial Regex MyRegex();
        
        [GeneratedRegex(@"^\d{2,3}$")]
        private static partial Regex MyRegex1();
    }
}