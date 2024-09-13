using System.Text.RegularExpressions;

namespace CalculadoraAOTY
{
    internal abstract partial class Program
    {
        private static readonly char[] Separator = ['\r', '\n'];
        private static readonly string[] SkipString = ["Rate", "feat.", "with", "\u00a9", "Save", "Updated"];

        private static void Main()
        {
            try
            {
                var texto = File.ReadAllText("ratings.txt");
                var crindex = texto.IndexOf('\u00a9');


                if (crindex == -1)
                {
                    Console.WriteLine("Erro: Caractere de copyright não encontrado no texto!");
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                    return;
                }

                var ratings = texto[crindex..];
                var lines = ratings.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                var scorepond = 0.0;
                var totalTimePond = 0.0;
                var totalSeconds = 0.0;
                var sumScore = 0.0;

                var index = 1;
                var cleanTime = "";
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();

                    var firstWord = trimmedLine.Split(" ").FirstOrDefault();
                    if (firstWord == null)
                        continue;

                    if (trimmedLine.Length > 3 && !SkipString.Any(s => firstWord.Contains(s)))
                    {
                        var match = MyRegex2().Match(trimmedLine);
                        
                        if (match.Success)
                        {
                            var precedingChar = match.Groups[1].Value[0];
                            var lastFiveChar = match.Groups[2].Value;
                            var firstDigit = match.Groups[2].Value[0];


                            cleanTime = lastFiveChar.Length > 4 && (precedingChar == ' ' || char.IsDigit(precedingChar) || firstDigit > '4')
                                ? MyRegex().Replace(trimmedLine[^4..], "")
                                : MyRegex().Replace(trimmedLine[^5..], "");



                            var seconds = int.Parse(cleanTime[^2..]);
                            var minutes = int.Parse(cleanTime[..^2].Replace(":", ""));
                            totalSeconds = seconds + minutes * 60;
                            totalTimePond += totalSeconds;
                        }
                        else
                        {
                            Console.WriteLine($"Erro: linha ({line}) não corresponde ao padrão esperado");
                            Console.WriteLine("Press Enter to exit...");
                            Console.ReadLine();
                            return;
                        }

                    }
                    else if (MyRegex1().IsMatch(trimmedLine))
                    {
                        var score = int.Parse(trimmedLine);
                        sumScore += score;
                        scorepond += score * totalSeconds;
                        Console.WriteLine($"{index} - tempo: {cleanTime} - score: {score}");
                        index++;
                    }



                }

                if (totalTimePond != 0)
                {
                    Console.WriteLine("--------------------------");
                    var pondAverage = Math.Round(scorepond / totalTimePond);
                    var average = Math.Round(sumScore / (index - 1));
                    Console.WriteLine($"Média: {average}");
                    Console.WriteLine($"Média Ponderada: {pondAverage}");
                }
                else
                {
                    Console.WriteLine("Erro: A Soma dos tempos da smúsicas é igual a 0!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar o arquivo: {ex.Message}");
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        [GeneratedRegex(@"^[^\d]*")]
        private static partial Regex MyRegex();

        [GeneratedRegex(@"^100|\d{1,2}$")]
        private static partial Regex MyRegex1();
        [GeneratedRegex(@"(.)(\d{1,2}\D\d{2})$")]
        private static partial Regex MyRegex2();
    }
}