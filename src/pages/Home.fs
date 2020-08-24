module pages.Home

open Fable.React
open Fable.React.Props
open Browser.Dom
open Browser.Types
open Fable.Core.JsInterop

importSideEffects "./Home.css"

let JSON =  Fable.Core.JS.JSON

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

type MayDangerChildren = R of ReactElement[] | S of string
 
type editor(?key, ?editable, ?style, ?ref, ?onInput, ?onFocus, ?children, ?className) =
  let editable = defaultArg editable false
  let style: CSSProp list = defaultArg style []
  let key = defaultArg key ""
  let className = defaultArg className ""
  let children: MayDangerChildren = defaultArg children (R [||])
  let attr: IHTMLProp list =
        [
          Key key
          ContentEditable editable
          Class ("editor " + className)
          Style style ]
  member this.render =
    let ref: IRefHook<Element option> = defaultArg ref Hooks.useRef None
    let loaded = Hooks.useRef false
    let handleInput (e: Browser.Types.Event) =
      let now = e.target :?> HTMLDivElement
      if onInput.IsSome then Option.get onInput now.innerHTML
    let handleFocus e =
      if onFocus.IsSome then Option.get onFocus e
    
    Hooks.useEffect((fun () ->
      loaded.current <- true
    ), [||])

    let attr = attr @ [
      RefHook ref
      OnInput handleInput
    ]
    match children with
    | R c -> div ( Seq.ofList attr ) c
    | S s -> div ( Seq.ofList (attr @ [ OnFocus handleFocus; DangerouslySetInnerHTML { __html = s }  ])) []

//type editProps = {
//  value: IStateHook<string>
//  onInput: option<string -> unit>
//}

let outerArea = FunctionComponent.Of(fun (props: {| children: ReactElement[]; onInput: option<string -> unit> |}) ->
  editor(
    key = "editArea",
    onInput = Option.get props.onInput,
    children = R props.children,
    className =  "editor body",
    style=[BackgroundColor "#acd"]).render
)

let spanArea = FunctionComponent.Of(fun (props: {|
                                                  value: string
                                                  editable: bool option
                                                  onFocus: option<FocusEvent -> unit>
                                                  |}) ->
  editor(
    key = "spanArea",
    ?editable = props.editable,
    children = S props.value,
    ?onFocus = props.onFocus
  ).render)

let testsLines = Tests.target.Split('\n') |> Array.toList

let ViewComponent = FunctionComponent.Of(fun () -> 
  let lineId = Hooks.useState(0)
  let lines = Hooks.useState(testsLines)
  let handleInput (s:string) =
    let replace id value =
      if id = lineId.current then s
      else value
    List.mapi replace lines.current |> lines.update
    
  let focusNode = Hooks.useState<HTMLDivElement>(null)
  let handleFocus (l: int) (e: Browser.Types.FocusEvent) =
    focusNode.update (e.target :?> HTMLDivElement)
    lineId.update l
    
  Hooks.useEffect((fun () ->
    let select = window.getSelection()
    console.log(select.anchorNode, select.anchorOffset, lineId.current)
  ), [|focusNode|])
  
  let rightContent = lines.current
                        |> List.map(fun s ->
                         spanArea {| value = s; onFocus = None; editable = Some false |})
                        |> Array.ofList
  let leftContent = testsLines |> List.mapi(fun line s ->
    spanArea {| value = s; onFocus = Some(handleFocus line); editable = Some true |})
                        |> Array.ofList
   
  block [
    outerArea {| children = leftContent; onInput = Some(handleInput) |}
    outerArea {| children = rightContent; onInput = None |}
  ]
)

let view = ViewComponent()


//      if ref.current.IsSome then
//        let now = ref.current.Value :?> HTMLDivElement
//        if onInput.IsSome then Option.get onInput now.innerHTML
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
