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
---

# 2. Verbindliche Arbeitsregeln

Diese Regeln gelten für die Weiterentwicklung dieses Repositories:

- Die Roadmap ist die verbindliche Source of Truth.
- Vor jeder Arbeit werden zuerst das aktuelle Repository und anschließend die vollständige aktuelle `roadmap.md` gelesen.
- Vor Änderungen wird der aktuelle Stand aller betroffenen Dateien gelesen.
- Bestehende Architektur, Helper, Konventionen und Workflows werden wiederverwendet.
- Externe APIs, Programme, Konfigurationsformate und Windows-Einstellungen werden nicht anhand von Vermutungen implementiert, sondern bei Bedarf anhand aktueller Primärquellen verifiziert.
- Änderungen müssen reproduzierbar sein.
- Ein Arbeitspaket wird erst nach einem praktischen Test als abgeschlossen markiert.
- Nach Änderungen wird der komplette betroffene Code auf Fehler geprüft.
- Neue dauerhafte Architekturentscheidungen werden in Abschnitt 3 dokumentiert.
- Verworfene Ansätze werden dokumentiert, damit sie nicht ohne neuen technischen Grund erneut verfolgt werden.

## Repository-Änderungen

- Es wird niemals direkt durch die KI in das GitHub-Repository geschrieben, committed, gepusht oder ein Pull Request erstellt.
- Änderungen werden als herunterladbarer `.ps1`-Patch bereitgestellt.
- Patches müssen defensiv arbeiten und dürfen einen unerwarteten Dateistand nicht stillschweigend überschreiben.
- Patches werden mit prozesslokalem Execution-Policy-Bypass ausgeführt:

  `pwsh -NoProfile -ExecutionPolicy Bypass -File "<Pfad-zum-Patch.ps1>"`

- `Set-ExecutionPolicy` wird nicht verwendet, solange diese Roadmap dies nicht ausdrücklich anders festlegt.

---

# 3. Architektur- und Entscheidungsprotokoll

Hier werden nur Entscheidungen eingetragen, die tatsächlich getroffen und technisch begründet wurden.

## Bestehende Entscheidungen

- [x] Zielplattform ist Windows 11.
- [x] Wortmann ist der erste zu unterstützende Hersteller.
- [x] Lenovo wird erst später ergänzt.
- [x] Die initialen Benutzereingaben erfolgen über eine GUI.
- [x] Das Setup soll Neustarts selbstständig durchführen und danach automatisch fortgesetzt werden.
- [x] Softwareinstallationen sollen auf Deutsch erfolgen.
- [x] Repository-Änderungen durch die KI erfolgen ausschließlich über lokale `.ps1`-Patches, nicht durch direkte GitHub-Schreibzugriffe.

## Noch nicht entschieden

Die folgenden Punkte sind bewusst noch **keine** Architekturentscheidungen und müssen vor Implementierung anhand der Anforderungen bzw. aktueller Primärquellen konkretisiert werden:

- konkrete PowerShell-/GUI-Technologie
- Persistenzmechanismus für die Fortsetzung nach Neustarts
- Wortmann-Schnittstelle bzw. offizieller Mechanismus für Treibersuche und -download
- Softwarequellen und Silent-Installationsparameter
- Verfahren zur Temperaturmessung
- konkrete Stress-/Benchmark-Automatisierung
- Regeln und Umfang des Windows-Debloatings
- Mechanismus zum Setzen von Standardanwendungen
- Struktur und Werte der OEM-Informationen

## Verworfene Ansätze

Noch keine dokumentiert.

---

# 4. Arbeitspakete

Die Reihenfolge bildet die aktuellen Abhängigkeiten ab. `[x]` darf erst nach bestandenem praktischem Test gesetzt werden.

## 4.1 Projektbasis und Ausführungsmodell

- [ ] Minimalen Bootstrap-/Entry-Point festlegen und implementieren.
- [ ] Voraussetzungen und unterstützte Ausführungsumgebung erkennen und validieren.
- [ ] Logging- und Fehlerbehandlungsgrundlage festlegen und implementieren.
- [ ] Zustandsmodell für einmalige Ausführung und Fortsetzung nach Neustart entwerfen, anhand aktueller Windows-/PowerShell-Mechanismen verifizieren und implementieren.

### Akzeptanzkriterien

- Das Setup lässt sich reproduzierbar auf einem frisch installierten Windows 11 starten.
- Fehler werden nachvollziehbar protokolliert und führen nicht zu einem stillen Abbruch.
- Nach einem simulierten erforderlichen Neustart wird exakt der vorgesehene nächste Schritt fortgesetzt.
- Nach vollständigem Abschluss startet das Setup nicht erneut unbeabsichtigt.

## 4.2 Systemerkennung und Herstellerabstraktion

- [ ] Hersteller, Modell und relevante Geräteidentifikatoren zuverlässig ermitteln.
- [ ] Herstellerabstraktion so anlegen, dass Wortmann zuerst implementiert und Lenovo später ergänzt werden kann.
- [ ] Verhalten für nicht unterstützte Hersteller definieren.

### Akzeptanzkriterien

- Wortmann-Systeme werden eindeutig erkannt.
- Benötigte Identifikatoren werden korrekt ausgelesen.
- Nicht unterstützte Systeme führen zu einer verständlichen Meldung statt zu falschen Treiberaktionen.

## 4.3 Wortmann-Treiberworkflow

- [ ] Offiziellen aktuellen Wortmann-Mechanismus für Treibersuche und Download anhand von Primärquellen verifizieren.
- [ ] Treiber anhand der im System hinterlegten Seriennummer ermitteln.
- [ ] Downloads reproduzierbar durchführen und validieren.
- [ ] Treiberinstallation automatisieren.
- [ ] Erforderliche Neustarts in das zentrale Fortsetzungsmodell integrieren.

### Akzeptanzkriterien

- Auf einem realen unterstützten Wortmann-Testsystem werden die korrekten Treiber gefunden.
- Downloads und Installationen schlagen bei ungültigen oder fehlenden Daten kontrolliert fehl.
- Nach erforderlichen Neustarts wird die Installation korrekt fortgesetzt.
- Der getestete Wortmann-Treiberworkflow ist vollständig reproduzierbar.

## 4.4 Initiale GUI und Eingabemodell

- [ ] GUI-Technologie festlegen.
- [ ] Eingabe des gewünschten Computernamens integrieren.
- [ ] Eingabe der Gerätenummer für OEM-Informationen integrieren.
- [ ] Softwareauswahl mit den definierten Standardwerten integrieren.
- [ ] Eingaben validieren und als zentralen Setup-Zustand bereitstellen.

### Akzeptanzkriterien

- Vorausgewählt sind Adobe Reader, Google Chrome und 7-Zip.
- Optional auswählbar sind Firefox, Thunderbird, Office 365, G Data Anti Virus / Internet Security / Total Protection und G Data MES Client.
- Ungültige Eingaben verhindern den Start der eigentlichen Installation mit verständlicher Rückmeldung.
- Die erfassten Werte stehen allen nachfolgenden Schritten konsistent zur Verfügung.

## 4.5 Computername und OEM-Informationen

- [ ] Computernamen validieren und setzen.
- [ ] OEM-Felder und deren Quellen verbindlich definieren.
- [ ] Hersteller, Gerätenummer, Supportinformationen und weitere definierte OEM-Daten setzen.
- [ ] Neustartbedarf über das zentrale Fortsetzungsmodell behandeln.

### Akzeptanzkriterien

- Der gewünschte Computername ist nach Abschluss aktiv.
- Die festgelegten OEM-Informationen werden an den vorgesehenen Windows-Stellen korrekt angezeigt.
- Ein erforderlicher Neustart unterbricht den Workflow nicht.

## 4.6 Softwareinstallation

- [ ] Für jede Software offizielle bzw. freigegebene Bezugsquelle und aktuelle Silent-Installationsparameter verifizieren.
- [ ] Adobe Reader auf Deutsch automatisieren.
- [ ] Google Chrome auf Deutsch automatisieren.
- [ ] 7-Zip auf Deutsch automatisieren.
- [ ] Firefox optional auf Deutsch automatisieren.
- [ ] Thunderbird optional auf Deutsch automatisieren.
- [ ] Office 365 optional auf Deutsch automatisieren.
- [ ] G Data Anti Virus / Internet Security / Total Protection optional automatisieren.
- [ ] G Data MES Client über den bereitgestellten Installer optional automatisieren.
- [ ] Installergebnisse einheitlich protokollieren und validieren.

### Akzeptanzkriterien

- Nur die in der GUI ausgewählten optionalen Anwendungen werden installiert.
- Die vorausgewählten Anwendungen werden installiert, sofern sie nicht bewusst abgewählt werden können bzw. die spätere GUI-Entscheidung dies vorsieht.
- Installationen verwenden die vorgesehene deutsche Sprache.
- Wiederholtes Ausführen eines einzelnen Installationsschritts verursacht keinen unkontrollierten Doppelzustand.

## 4.7 Standardanwendungen

- [ ] Anforderungen festlegen, welche installierten Anwendungen für welche Dateitypen/Protokolle Standard werden sollen.
- [ ] Aktuell unterstützten Windows-11-Mechanismus anhand von Microsoft-Primärquellen verifizieren.
- [ ] Standardzuordnungen reproduzierbar anwenden.

### Akzeptanzkriterien

- Nur explizit definierte Zuordnungen werden verändert.
- Die Zuordnungen sind nach einem Neustart weiterhin wirksam.
- Nicht installierte optionale Anwendungen erzeugen keine ungültigen Zuordnungen.

## 4.8 Leichtes Windows-Debloating und Grundeinstellungen

- [ ] Konkrete Liste zu entfernender Windows-Komponenten/Apps festlegen.
- [ ] Jeden Eingriff auf Supportbarkeit und Nebenwirkungen prüfen.
- [ ] Freigegebene Debloat-Schritte implementieren.
- [ ] Taskleistensymbole links konfigurieren.

### Akzeptanzkriterien

- Es werden ausschließlich zuvor ausdrücklich freigegebene Komponenten entfernt.
- Windows Update und zentrale Systemfunktionen bleiben funktionsfähig.
- Die Taskleistensymbole sind links ausgerichtet.
- Die Schritte sind bei erneutem Aufruf kontrolliert/idempotent.

## 4.9 Stresstests, Benchmarks und Temperaturüberwachung

- [ ] Verteilung, Lizenz-/Nutzungsbedingungen und Automatisierbarkeit von Prime95 anhand aktueller offizieller Quellen prüfen.
- [ ] Verteilung, Lizenz-/Nutzungsbedingungen und Automatisierbarkeit von Cinebench anhand aktueller offizieller Quellen prüfen.
- [ ] Verfahren zur zuverlässigen Temperaturmessung festlegen und verifizieren.
- [ ] Prime95-Test mit Temperaturaufzeichnung automatisieren.
- [ ] Cinebench-Test mit Temperaturaufzeichnung automatisieren.
- [ ] Gaming-Grafikkarte zuverlässig erkennen.
- [ ] Geeigneten Gaming-Benchmark festlegen und dessen Automatisierbarkeit verifizieren.
- [ ] Gaming-Benchmark bei erkannter Gaming-GPU mit Temperaturaufzeichnung automatisieren.
- [ ] Einheitlichen Ergebnisbericht für Benchmarkwerte und Temperaturen erstellen.

### Akzeptanzkriterien

- Jeder Test beendet sich definiert und blockiert den Setup-Workflow nicht dauerhaft.
- Temperaturen werden während der jeweiligen Testlaufzeit aufgezeichnet.
- Ein Gaming-Benchmark läuft nur, wenn die dafür definierte GPU-Bedingung erfüllt ist.
- Der Abschlussbericht enthält die verfügbaren Benchmarkwerte und die zugehörigen Temperaturdaten.
- Fehler eines Tests werden nachvollziehbar ausgewiesen.

## 4.10 Gesamtintegration und Abschluss

- [ ] Alle Komponenten in eine reproduzierbare End-to-End-Reihenfolge integrieren.
- [ ] Verhalten bei Teilfehlern und Wiederaufnahme verbindlich festlegen.
- [ ] Abschlusszustand dauerhaft markieren.
- [ ] Temporäre Dateien und nur für den Lauf benötigte Zustände kontrolliert bereinigen.
- [ ] Vollständigen End-to-End-Test auf einem frisch installierten Windows-11-Testsystem durchführen.

### Akzeptanzkriterien

- Das Setup läuft vom ersten Start bis zum Abschluss ohne manuelle Skript-Neustarts durch.
- Notwendige Windows-Neustarts werden automatisch durchgeführt und korrekt fortgesetzt.
- Nach Abschluss werden die definierten Ergebnisse nachvollziehbar ausgegeben.
- Ein bereits erfolgreich abgeschlossenes Setup startet nicht versehentlich erneut.
- Alle zuvor als `[x]` markierten Teilkomponenten bleiben im Gesamttest funktionsfähig.

---

# 5. Arbeitsregel für den nächsten Schritt

Solange Abschnitt 4.1 nicht praktisch abgeschlossen ist, wird nicht mit nachgelagerten Implementierungen begonnen, außer eine dafür notwendige Recherche dient ausdrücklich der Entscheidung in 4.1.

Der nächste technische Arbeitsschritt ist daher:

1. aktuellen Repository-Stand lesen,
2. aktuelle Primärquellen für den vorgesehenen Windows-11-/PowerShell-Ausführungsrahmen prüfen,
3. das minimale Bootstrap-/Entry-Point-Konzept festlegen,
4. erst danach einen defensiven `.ps1`-Patch für dessen Implementierung erstellen,
5. praktisch testen,
6. anschließend erst die zugehörigen Roadmap-Punkte als `[x]` markieren.