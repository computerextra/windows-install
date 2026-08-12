# Windows Install Roadmap

> **Zweck dieses Dokuments**
>
> Diese Roadmap ist nicht nur eine Checkliste, sondern die **Arbeits- und Entscheidungsgrundlage für eine KI**, die das Repository weiterentwickelt.
>
> Eine KI soll nach dem Einlesen dieses Dokuments verstehen:
>
> 1. welches Zielbild für das Windows-System verfolgt wird,
> 2. welche Komponenten bereits umgesetzt und getestet sind,
> 3. welche Entscheidungen bereits bewusst getroffen wurden,
> 4. welche Ansätze verworfen wurden und nicht erneut verfolgt werden sollen,
> 5. welche Arbeiten noch offen sind,
> 6. welche Abhängigkeiten zwischen den Arbeitspaketen bestehen,
> 7. wie aus dem aktuellen Stand selbstständig die sinnvollsten nächsten Schritte abzuleiten sind,
> 8. welche Akzeptanzkriterien erfüllt sein müssen, bevor ein Punkt als abgeschlossen gilt.
>
> **Wichtig:** Bestehende, als `[x]` markierte Entscheidungen sollen nicht ohne konkreten technischen Grund zurückgebaut oder durch alternative Ansätze ersetzt werden.

---

# 1. Gesamtziel

Ein vollständig automatisiertes und reproduzierbares Windows-11-Setup für:

- Script wird direkt nach neuinstallation von Windows ausgeführt
- Erkennung des Herstellers des Systems
- Automatische Treibersuche mit Download
    - Wir beginnen mit Wortmann (Hier ist die Seriennummer direkt im System hinterlegt, mit der Seriennummer können Treiber installiert werden)
    - Lenovo wird später hinzugefügt
- Automatische installation von Treibern
- Automatischer Neustart des Systems und weiterführung des Skripts
- Integration von stresstests für das system
    - Prime95 mit Temperatur überwachung
    - Cinebench mit Temperatur überwachung
    - Wenn Gaming Grafikkarte: Gaming Benchmark mit Temperatur überwachung
    - Nach allen Tests: Ausgabe der Benchmark Werte sowie der Temperaturen während des Tests
- Installation von Software nach Benutzerwahl
    - Adobe Reader (vorausgewählt)
    - Google Chrome (vorausgewählt)
    - Adobe Firefox (optional)
    - 7Zip (vorausgewählt)
    - Thunderbird (optional)
    - Office 365 (optional)
    - G Data Anti Virus / Internet Security / Total Protection (optional)
    - G Data MES Client (optional) (eigener Installer, wird bereitgestellt)
    - Alle installationen immer auf Deutsch!
- Anpassung des Computer Namens
- Installiert Software als Standard setzen
- Integration von OEM Informationen
    - Hersteller
    - Gerätenummer
    - Support Nummern
    - etc.
- Die initialen Einstellungen sollen durch eine GUI abgefragt werden.
    - Auswahl der zu installierenden Software
    - Eingabe der "Gerätenummer für OEM Informationen"
    - Eingabe des gewünschten Computer Namens
- Leichter Debloat von Windows (nicht benötigte Software wird entfernt)
- initiale Einstellungen am Windows
    - Taskleistensymbole links
- Das script soll einmalig ausgeführt werden
    - Falls das system neugestartet werden muss, soll der neustart automatisch durchgeführt werden
    - Das script soll nach dem neustart automatsch fortgesetzt werden.
