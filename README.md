Ce repo contient tout ce qu'il faut pour lancer le bouzin, y compris un dump de la base de données avec son schéma.

Il manque juste un fichier ```Config.fs``` dans le projet ```Gepetto``` qui doit avoir la forme suivante :

```F#
namespace Gepetto

module Config =
    [<Literal>]
    let dbString = "Le string de connexion à la base de données"
```
