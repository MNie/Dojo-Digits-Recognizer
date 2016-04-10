open System
open System.IO

let trainingData = File.ReadAllLines(@"trainingsample.csv")
let validationData = File.ReadAllLines(@"validationsample.csv")

type Example = { Label:int; Pixels:int[] }

let trainArrayOfString = 
    trainingData.[ 1 ..]
    |> Array.Parallel.map (fun s -> s.Split(','))
    |> Array.Parallel.map (Array.Parallel.map int)
    |> Array.Parallel.map (fun s -> { Label = s.[0]; Pixels = s.[1..]})
 
let validationArrayOfString =
    validationData.[ 1 ..]
    |> Array.Parallel.map (fun s -> s.Split(','))
    |> Array.Parallel.map (Array.Parallel.map int)
    |> Array.Parallel.map (fun s -> { Label = s.[0]; Pixels = s.[1..]})

let distance (p1: int[]) (p2: int[]) = 
    Array.map2 (fun x y -> Math.Abs ((float)(x - y)) ) p1 p2
    |> Array.sum

let correctAnswer (p2: Example) = 
    trainArrayOfString
    |> Array.minBy (fun x -> distance x.Pixels p2.Pixels)
    |> fun x -> x.Label
 
let sumOfCorrect = 
    validationArrayOfString 
    |> Array.Parallel.map correctAnswer 
    |> fun s -> Array.map2 (fun p1 p2 -> if(p1 = p2.Label) then 1 else 0) s validationArrayOfString 
    |> Array.sum 
    |> float

let percentOfCorrectness = 
    sumOfCorrect / (float)validationArrayOfString.Length 
    |> fun x -> x*100.0 
    |> fun y -> y.ToString() + "%"