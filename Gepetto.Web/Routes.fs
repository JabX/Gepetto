namespace Gepetto.Web

open WebSharper.Sitelets

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/search">] Search