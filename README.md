Projekt został stworzony w ramach przedmiotu Programowanie Równoległe i Rozproszone na kierunku Informatyka Stosowana w semestrze 6 na Wydziale Elektrycznym Politechniki Warszawskiej

## Temat Projektu
Tematem projektu jest utworzenie trójwymiarowego automatu komórkowego, wokół którego można się przemieszczać, obracać i przybliżać (wykorzystanie wirtualnej kamery - własna implementacja), a następnie jego optymalizacja pod kątem obliczeń równoległych.

## Demonostracja działania
Demonostracja została utworzona dla świata o wymiarach 35x35x35 sześcianów.
![Demo](Media/2023-06-29%2022-18-21.mp4)

## Technologia
Projekt został napisany w języku C# z użyciem biblioteki AvaloniaUI tworzącej okno aplikacji.

## Sposób wykonania
Scena złożona jest ze sklejonych ze soba sześcianów (Domyślnie 30x30x30 sześcianów - możliwa zmiana wielkości świata w kodzie)


## Sterowanie 
| Przemieszczanie obserwatora | Przypisany klawisz |
| --- | --- |
| Do przodu | E |
| Do tyłu | D |
| W lewo | S |
| W prawo | F |
| W górę | T |
| W dół | G |

| Obracanie obserwatora | Przypisany klawisz |
| --- | --- |
| W lewo | J |
| W prawo | L |
| W górę | I |
| W dół | K |
| Zgodnie z ruchem wskazówek zegara | O |
| Przeciwnie do ruchu wskazówek zegara | U |

| Przybliżanie i oddalanie | Przypisany klawisz |
| --- | --- |
| Przybliżanie | R |
| Oddalenie | W |


## Uruchomienie
```dotnet
dotnet run --project guiProject/guiProject.Desktop
```

## Autorzy
- Jakub Maciejewski
- Jakub Goliszewski

