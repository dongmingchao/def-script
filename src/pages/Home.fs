module pages.Home

open Fable.React
open Fable.React.Props
open Browser.Dom
open Browser.Types
open Fable.Core.JsInterop

importSideEffects "./Home.css"

let block = div [ Style [ Display DisplayOptions.Flex; ] ]

type EditorProps =
    { style: list<CSSProp> option
      editable: bool option }
type State = { Count: int }

// type editor (props) =
//   inherit Component<EditorProps, State>(props)
//   let editable = defaultArg props.editable false
//   let style = defaultArg props.style []
//   let ref = Hooks.useRef None
//   let handleFocus (e: Browser.Types.FocusEvent) = console.log e
//   let handleInput (e: Browser.Types.Event) = console.log(e, ref)
//   let handleChange e = console.log e
//   let attr: IHTMLProp list =
//         [ ContentEditable editable
//           RefHook ref
//           OnFocus handleFocus
//           OnInput handleInput
//           OnChange handleChange
//           Style (List.concat [ style; [Width "50%"; MinHeight "200px"] ]) ]
//   override this.render() = div ( Seq.ofList attr) []

type TypeElement =
  abstract render: _ -> ReactElement

//let spanArea = FunctionComponent.Of(fun () ->
//  span [ ContentEditable true; Style [BackgroundColor "red"] ] []
//)
  
type editor(?key, ?editable, ?style, ?ref, ?onInput, ?children, ?className) =
  let editable = defaultArg editable false
  let style: CSSProp list = defaultArg style []
  let key = defaultArg key ""
  let className = defaultArg className ""
  let children: ReactElement[] = defaultArg children [||]
  let handleFocus (e: Browser.Types.FocusEvent) =
    console.log (e, window.getSelection())
  let attr: IHTMLProp list =
        [
          Key key
          ContentEditable editable
          OnFocus handleFocus
          Class ("editor " + className)
          Style style ]
  member this.render =
    let ref: IRefHook<Element option> = defaultArg ref Hooks.useRef None
//    let mutable offset = window.getSelection().anchorOffset
//    let mutable focusNode = window.getSelection().focusNode
    let handleInput (e: Browser.Types.Event) =
      if ref.current.IsSome then
        let now = ref.current.Value :?> HTMLDivElement
        if onInput.IsSome then Option.get onInput now.innerHTML
//        let sel = window.getSelection()
//        offset <- sel.anchorOffset
//        focusNode <- sel.focusNode
//        let att = focusNode.attributes.getNamedItem("contenteditable")
//        att.value <- "true"
//        focusNode.attributes.setNamedItem(att) |> ignore

//    Hooks.useEffect((fun() ->
//      if editable then
//        if (not(isNull focusNode)) then
//          console.log("collapse", focusNode, offset)
//          window.getSelection().collapse(focusNode, offset)
//          window.setInterval((fun () -> console.log(focusNode, offset)), 1000) |> ignore
//      ), [|value.current|])

    let attr = attr @ [
      RefHook ref
      OnInput handleInput
    ]
    div ( Seq.ofList attr ) children

//type editProps = {
//  value: IStateHook<string>
//  onInput: option<string -> unit>
//}

let outerArea = FunctionComponent.Of(fun (props: {| children: ReactElement[]; onInput: option<string -> unit> |}) ->
  editor(
    key = "editArea",
    onInput = Option.get props.onInput,
    children = props.children,
    editable= false,
    className =  "editor body",
    style=[BackgroundColor "#acd"]).render
)

let spanArea = FunctionComponent.Of(fun (props: {| value: string |}) ->
  editor(
    key = "spanArea",
    editable = true,
    children = [| str props.value |]
  ).render)

let target = Tests.target.Split('\n') 
          |> Array.map(fun s -> spanArea {| value = s |})

let ViewComponent = FunctionComponent.Of(fun () -> 
//  let state = Hooks.useState(target)
//  let handleInput (s:string) =
//    state.update s
   
  block [
    outerArea {| children = target; onInput = None |}
    outerArea {| children = target; onInput = None |}
  ]
)

let view = ViewComponent()