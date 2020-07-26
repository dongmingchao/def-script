module pages.Home

open Fable.React
open Fable.React.Props
open Browser.Dom
open Browser.Types

let block = div [ Style [ Display DisplayOptions.Flex ] ]

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
  
type editor(?key, ?editable, ?style, ?ref, ?onInput, ?value) =
  let editable = defaultArg editable false
  let style = defaultArg style []
  let key = defaultArg key ""
  let handleFocus (e: Browser.Types.FocusEvent) = console.log (e, window.getSelection())
  let attr: IHTMLProp list =
        [
          Key key
          ContentEditable editable
          OnFocus handleFocus
          Style (List.concat [ style; [Width "50%"; MinHeight "200px"] ]) ]
  member this.render =
    let value = defaultArg value (Hooks.useState "")
    let ref: IRefHook<Element option> = defaultArg ref Hooks.useRef None
    let mutable offset = window.getSelection().anchorOffset
    let handleInput (e: Browser.Types.Event) =
      if ref.current.IsNone then ()
      else
        let now = ref.current.Value :?> HTMLDivElement
        if onInput.IsSome then Option.get onInput now.innerHTML
        offset <- window.getSelection().anchorOffset
        console.log now.innerHTML

    Hooks.useEffect((fun() ->
      if editable then
        let sel = window.getSelection()
        if (not(isNull sel.anchorNode)) then 
          sel.collapse(sel.focusNode.firstChild, offset)
      ), [|value.current|])

    let attr = attr @ [
      RefHook ref
      OnInput handleInput
      DangerouslySetInnerHTML { __html = value.current }
    ]
    div ( Seq.ofList attr ) []

type areaProps = {
  value: IStateHook<string>
  onInput: option<string -> unit>
}

let nothingFunc _ = ()

let editArea = FunctionComponent.Of(fun (props: areaProps) ->
  let onInput = defaultArg props.onInput nothingFunc
  editor(
    key = "editArea",
    value = props.value,
    onInput = onInput,
    editable=true,
    style=[BackgroundColor "#acd"]).render
)

let showArea = FunctionComponent.Of(fun (props: areaProps) ->
  editor(
    key = "showArea",
    value = props.value,
    editable=false,
    style=[BackgroundColor "#acc"]).render
)

let ViewComponent = FunctionComponent.Of(fun () -> 
  let state = Hooks.useState("123")
  let handleInput (s:string) =
    console.log ("out input", s)
    state.update s
   
  block [
    editArea { value = state; onInput = Some(handleInput) }
    showArea { value = state; onInput = None }
  ]
)

let view = ViewComponent()