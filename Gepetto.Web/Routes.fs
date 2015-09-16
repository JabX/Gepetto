namespace Gepetto.Web

open WebSharper.Sitelets

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/search">] Search
    | [<EndPoint "/word">] Word of int