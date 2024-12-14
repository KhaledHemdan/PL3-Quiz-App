open System
open System.Windows.Forms

// Define the record type for questions
type Question = {
    Text: string
    Options: string[] option // Use option type for Options, as written questions do not have options
    CorrectAnswer: string
    IsMultipleChoice: bool // Boolean to check if it's a multiple-choice question
}

// Quiz Questions
let quizQuestions = 
    [|
        { Text = "What is 2 + 2?"; Options = Some [| "3"; "4"; "5"; "6" |]; CorrectAnswer = "4"; IsMultipleChoice = true }
        { Text = "Who wrote 'Macbeth'?"; Options = Some [| "Shakespeare"; "Dickens"; "Hemingway"; "Austen" |]; CorrectAnswer = "Shakespeare"; IsMultipleChoice = true }
        { Text = "What is the capital of France?"; Options = Some [| "Berlin"; "Madrid"; "Paris"; "Rome" |]; CorrectAnswer = "Paris"; IsMultipleChoice = true }
        { Text = "Describe your favorite holiday."; Options = None; CorrectAnswer = "My favorite holiday is Christmas."; IsMultipleChoice = false }
    |]

let mutable userAnswers = []

// Create the form and controls
let form = new Form(Text = "Quiz Application", Width = 500, Height = 500)
let questionLabel = new Label(Top = 20, Left = 20, Width = 400, Height = 50)
let answerBox = new TextBox(Top = 80, Left = 20, Width = 300)
let optionsPanel = new FlowLayoutPanel(Top = 80, Left = 20, Width = 300, Height = 200)
let nextButton = new Button(Text = "Next", Top = 300, Left = 50, Width = 100)
let submitButton = new Button(Text = "Submit", Top = 300, Left = 160, Width = 100)
submitButton.Enabled <- false

// Add controls to the form
form.Controls.Add(questionLabel)
form.Controls.Add(answerBox)
form.Controls.Add(optionsPanel)
form.Controls.Add(nextButton)
form.Controls.Add(submitButton)

// State management
let mutable currentIndex = 0

// Update UI function
let updateUI () =
    let currentQuestion = quizQuestions.[currentIndex]
    questionLabel.Text <- currentQuestion.Text
    
    if currentQuestion.IsMultipleChoice then
        optionsPanel.Controls.Clear()
        currentQuestion.Options |> Option.iter (fun options ->
            options |> Array.iter (fun option ->
                let radioButton = new RadioButton(Text = option, Width = 200)
                optionsPanel.Controls.Add(radioButton)
            )
        )
        answerBox.Visible <- false
        optionsPanel.Visible <- true
    else
        optionsPanel.Controls.Clear()
        answerBox.Visible <- true
        answerBox.Text <- ""
        optionsPanel.Visible <- false

updateUI()

// Capture user answers
nextButton.Click.Add(fun _ ->
    let currentQuestion = quizQuestions.[currentIndex]
    
    let userAnswer =
        if currentQuestion.IsMultipleChoice then
            optionsPanel.Controls
            |> Seq.cast<RadioButton>
            |> Seq.tryFind (fun rb -> rb.Checked)
            |> Option.map (fun rb -> rb.Text)
            |> Option.defaultValue ""
        else
            // For written questions, get the text input from the TextBox
            answerBox.Text
    
    userAnswers <- userAnswers @ [userAnswer]
    currentIndex <- currentIndex + 1
    if currentIndex < quizQuestions.Length then
        updateUI()
    else
        nextButton.Enabled <- false
        submitButton.Enabled <- true
)

// Submit and calculate score
submitButton.Click.Add(fun _ ->
    let score =
        Array.zip (userAnswers |> List.toArray) quizQuestions
        |> Array.sumBy (fun (userAnswer, question) ->
            if userAnswer.Trim().ToLower() = question.CorrectAnswer.Trim().ToLower() then 1 else 0
        )

    MessageBox.Show(sprintf "Quiz Complete! Your score is %d out of %d" score quizQuestions.Length) |> ignore
)



// Run the application
[<STAThread>]
Application.Run(form)
