// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open FSharp.Charting
open System.Windows.Forms
open System.Threading
open System.Collections.ObjectModel

let chartFunction step (fromX, toX) f =
    seq {for x in {fromX .. step .. toX} -> (x, f x) }
        |> Chart.Line

let ChartFunction (fromX, toX) f = 
    let step = (toX - fromX) / 1000.
    chartFunction step (fromX, toX) f

let updatable = 
    let feed = (seq { let x=ref 0. in while true do x:=!x+1.; yield !x }).GetEnumerator()
    let collection=new ObservableCollection<float*float>();    
    async { 
        while feed.MoveNext() do            
            do! Async.Sleep 300                   
            let value=feed.Current            
            (value, value+1.) |> collection.Add
    } |> Async.StartImmediate    
    collection

let randSeq = (seq { let rand = new System.Random() in while true do yield (System.Math.Round(rand.NextDouble(),1), rand.NextDouble())}).GetEnumerator()

let sinSeq= (seq { let x=ref 0. in while true do yield (!x, System.Math.Sin(!x)); x := !x+0.1;}).GetEnumerator()

[<EntryPoint>]
let main argv =       
    //let chart = (ChartFunction (-10., 10.) (fun x -> cos x)).WithXAxis(Title = "Domain", Min = -10., Max = 10.)  
    let collection=new ObservableCollection<float*float>()
    let chart = Chart.Point(collection, Color=System.Drawing.Color.Red)
    let chartForm = chart.ShowChart();
    async {
        let sequence=randSeq
        while sequence.MoveNext() do
            collection.Add(sequence.Current)           
            do! Async.Sleep(100)                   
    } |> Async.StartImmediate              
    //chartForm.Resize.Add(fun args -> chartForm.PerformAutoScale())  
    Application.Run(chartForm)   

   

    0 // return an integer exit code


