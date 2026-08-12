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

## Verbindliche Qualitäts- und Testanforderungen

- Jede neu implementierte Logik erhält passende automatisierte Tests.
- Änderungen dürfen nicht allein aufgrund erfolgreicher Kompilierung als ausreichend geprüft gelten.
- GitHub Actions dient als verbindliches Qualitätsgate für Build, automatisierte Tests und statische Prüfungen.
- Wo reale Windows- oder Hardwareeffekte in CI nicht direkt ausführbar sind, muss die Architektur testbare Abstraktionen verwenden, damit Entscheidungslogik, Zustandsübergänge, Fehlerfälle und externe Interaktionen automatisiert geprüft werden können.
- Für kritische Workflows werden zusätzlich Integrations- bzw. Workflowtests mit kontrollierten Test-Doubles/Fixtures vorgesehen.
- Ein Build, der erforderliche Tests oder Prüfungen nicht besteht, darf nicht als freizugebendes Installationsartefakt behandelt werden.
- Praktische Hardwaretests erfolgen erst mit einem dafür geeigneten Stand auf einem frisch installierten Wortmann-Gerät.
- Der erste reale Hardwaretest ersetzt die automatisierten Tests nicht, sondern ergänzt sie.
- Lokale Tests auf dem Windows-Entwicklungsrechner sind ausdrücklich vorgesehen, solange sie keine persistenten Systemänderungen auslösen.
- Die GUI darf lokal vollständig gestartet und geprüft werden, sofern sie im Entwicklungsmodus ausschließlich nicht-mutuierende bzw. simulierte Backends verwendet.

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

### Verbindliche, noch nicht praktisch abgenommene Architekturvorgaben

- Das fertige Setup muss ohne vorheriges Klonen oder manuelles Herunterladen des Repositorys über einen einzelnen Startbefehl auf einem frisch installierten Windows-11-System startbar sein.
- PowerShell ist keine vorgeschriebene Implementierungssprache. Andere geeignete Technologien sind zulässig.
- Falls Komponenten kompiliert werden, müssen die ausführbaren Artefakte reproduzierbar über GitHub Actions gebaut werden; das Zielsystem benötigt keine lokale Entwicklungs- oder Build-Umgebung.
- Die Herstellerintegration muss über eine erweiterbare Abstraktion erfolgen. Der zentrale Workflow darf nicht fest auf einzelne Hersteller verdrahtet werden.
- Wortmann wird als erster Hersteller vollständig implementiert und praktisch getestet.
- Nach Wortmann sind Lenovo, Asus und Acer als weitere Hersteller vorgesehen und müssen später als zusätzliche Implementierungen derselben Herstellerabstraktion ergänzt werden können.
- Die Wortmann-Unterstützung ist geräteklassenunabhängig auszulegen: Notebook, Desktop-PC, Tablet und All-in-One müssen über denselben generischen Herstellerworkflow unterstützt werden.
- Die Treiberermittlung für Wortmann erfolgt anhand der vom Hersteller im System hinterlegten Seriennummer. Gerätemodell oder Geräteklasse dürfen nicht als primärer Schlüssel für die Treibersuche vorausgesetzt werden.
- Neue Logik wird grundsätzlich testbar entworfen; Hardware-, Netzwerk-, Prozess-, Registry-, WMI/CIM- und ähnliche Systemzugriffe werden so gekapselt, dass die fachliche Logik automatisiert getestet werden kann.
- Für die erste Implementierungsbasis wird C# auf .NET 10 verwendet; die GUI wird mit WPF umgesetzt.
- Lokale Entwicklung erfolgt mit .NET SDK 10.0.400 oder einem kompatiblen Patch derselben Feature-Band gemäß `global.json`.
- MSTest.Sdk 4.1.0 ist die initiale Testplattform.
- Lokale Builds, automatisierte Tests und die GUI dürfen auf dem Entwicklungsrechner ausgeführt werden. Persistente Windows-Systemänderungen sind dort nicht zulässig und müssen durch eine explizite Entwicklungs-Sicherheitsgrenze blockiert werden.
- Der aktuelle Computername wird beim Öffnen der GUI automatisch aus dem System gelesen und als vorbelegter Wert angezeigt.
- Die Gerätenummer für OEM-Informationen ist optional.
- Wenn keine Gerätenummer angegeben ist, werden keine OEM-Informationen geschrieben oder verändert.
- Wenn bereits eine Gerätenummer in den vorgesehenen OEM-Informationen hinterlegt ist, wird sie beim Öffnen der GUI automatisch ausgelesen und vorbelegt.
- Bei erneutem Start der GUI muss der bestehende Systemzustand berücksichtigt werden: bereits installierte Software wird erkannt und in der GUI automatisch als ausgewählt dargestellt.
- Bei erneutem Start muss nachvollziehbar sein, ob der Treiberworkflow bereits erfolgreich durchgeführt wurde.
- Wird bei einem späteren erneuten Lauf erstmals eine Gerätenummer angegeben, werden die OEM-Informationen in diesem Lauf geschrieben, ohne bereits erfolgreich abgeschlossene andere Schritte unnötig erneut auszuführen.
- Unsichtbare, projektspezifische Marker dürfen verwendet werden, um vom Installer selbst abgeschlossene Workflow-Schritte nachvollziehbar zu machen.
- Marker sind nicht die alleinige Wahrheitsquelle für tatsächlich vorhandene Software oder sichtbare OEM-Daten: installierte Software und bereits vorhandene OEM-Informationen müssen soweit technisch zuverlässig möglich aus dem realen Systemzustand erkannt werden.
- Der konkrete Speicherort, das Schema, die Versionierung und die Integritätsregeln für Marker werden vor Implementierung anhand geeigneter Windows-Mechanismen festgelegt und dokumentiert.

## Noch nicht entschieden

Die folgenden Punkte sind bewusst noch **keine** Architekturentscheidungen und müssen vor Implementierung anhand der Anforderungen bzw. aktueller Primärquellen konkretisiert werden:

- konkrete Implementierungssprache(n), Runtime(s) und GUI-Technologie
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

- [ ] Minimalen Bootstrap-/Entry-Point für den Aufruf über einen einzelnen Befehl festlegen und implementieren.
- [ ] Geeignete Implementierungstechnologie(n) anhand der Anforderungen auswählen; PowerShell ist eine Option, aber keine Vorgabe.
- [ ] Download-/Startmechanismus so gestalten, dass kein vorheriges Klonen oder manuelles Herunterladen des Repositorys erforderlich ist.
- [ ] Falls kompilierte Komponenten eingesetzt werden, GitHub-Actions-Workflow für reproduzierbare Builds und die Bereitstellung der benötigten Artefakte definieren und implementieren.
- [ ] Voraussetzungen und unterstützte Ausführungsumgebung erkennen und validieren.
- [ ] Logging- und Fehlerbehandlungsgrundlage festlegen und implementieren.
- [ ] Testarchitektur und Abstraktionen für externe Windows-/Hardwarezugriffe festlegen und implementieren.
- [ ] Zustandsmodell für einmalige Ausführung und Fortsetzung nach Neustart entwerfen, anhand aktueller Windows-Mechanismen verifizieren und implementieren.
- [ ] Marker-/Statusmodell für bereits erfolgreich abgeschlossene Workflow-Schritte entwerfen, versionieren und testbar abstrahieren.
- [ ] Regeln definieren, wie Marker mit real erkanntem Systemzustand abgeglichen werden, damit veraltete Marker keine falschen Aussagen über installierte Software oder sichtbare Konfiguration erzeugen.

### Akzeptanzkriterien

- Das Setup lässt sich reproduzierbar auf einem frisch installierten Windows 11 mit einem einzelnen Befehl starten.
- Vor dem Start sind weder `git clone` noch ein manuelles Herunterladen des Projekts erforderlich.
- Auf dem Zielsystem ist keine Entwicklungs- oder Build-Umgebung erforderlich.
- Werden kompilierte Komponenten verwendet, stammen die ausgeführten Binärartefakte aus dem definierten GitHub-Actions-Build.
- Automatisierte Tests für Bootstrap-/Workflowlogik, Zustandsübergänge, Fehlerbehandlung und testbar gekapselte Systemzugriffe laufen in GitHub Actions erfolgreich.
- Fehler werden nachvollziehbar protokolliert und führen nicht zu einem stillen Abbruch.
- Nach einem simulierten erforderlichen Neustart wird exakt der vorgesehene nächste Schritt fortgesetzt.
- Nach vollständigem Abschluss startet das Setup nicht erneut unbeabsichtigt.

## 4.2 Systemerkennung und Herstellerabstraktion

- [ ] Hersteller, Modell, Geräteklasse und relevante Geräteidentifikatoren zuverlässig ermitteln.
- [ ] Herstellerabstraktion so anlegen, dass Wortmann zuerst implementiert und Lenovo, Asus sowie Acer später ohne Änderung des zentralen Workflows ergänzt werden können.
- [ ] Herstellerabhängige Logik hinter klaren Schnittstellen kapseln und mit Test-Doubles automatisiert testbar machen.
- [ ] Verhalten für nicht unterstützte Hersteller definieren.

### Akzeptanzkriterien

- Wortmann-Systeme werden unabhängig von der Geräteklasse eindeutig erkannt.
- Notebook, Desktop-PC, Tablet und All-in-One durchlaufen dieselbe Herstellerabstraktion.
- Benötigte Identifikatoren werden korrekt ausgelesen.
- Die Herstellerabstraktion lässt sich in automatisierten Tests mit Wortmann-, Lenovo-, Asus-, Acer- und unbekannten Hersteller-Fixtures prüfen, auch wenn zunächst nur Wortmann produktiv implementiert ist.
- Nicht unterstützte Systeme führen zu einer verständlichen Meldung statt zu falschen Treiberaktionen.

## 4.3 Wortmann-Treiberworkflow

- [ ] Offiziellen aktuellen Wortmann-Mechanismus für Treibersuche und Download anhand von Primärquellen verifizieren.
- [ ] Treiber anhand der im System hinterlegten Seriennummer ermitteln.
- [ ] Downloads reproduzierbar durchführen und validieren.
- [ ] Treiberinstallation automatisieren.
- [ ] Erforderliche Neustarts in das zentrale Fortsetzungsmodell integrieren.

### Akzeptanzkriterien

- Die Seriennummer wird auf Wortmann-Systemen aus der vorgesehenen systemseitigen Quelle zuverlässig ausgelesen.
- Die Treibersuche verwendet bei Wortmann die Seriennummer als primären Schlüssel.
- Der Wortmann-Treiberworkflow enthält automatisierte Tests für gültige Seriennummern, fehlende/ungültige Seriennummern, leere Treffer, Mehrfachtreffer, Downloadfehler und Validierungsfehler.
- Auf einem realen unterstützten Wortmann-Testsystem werden die korrekten Treiber gefunden.
- Der reale Test darf ein Notebook, Desktop-PC, Tablet oder All-in-One sein; die Implementierung selbst darf keine dieser Geräteklassen voraussetzen oder ausschließen.
- Downloads und Installationen schlagen bei ungültigen oder fehlenden Daten kontrolliert fehl.
- Nach erforderlichen Neustarts wird die Installation korrekt fortgesetzt.
- Der getestete Wortmann-Treiberworkflow ist vollständig reproduzierbar.

## 4.4 Initiale GUI und Eingabemodell

- [ ] GUI-Technologie festlegen.
- [ ] Aktuellen Computernamen beim Öffnen der GUI auslesen und als vorbelegten Wert anzeigen.
- [ ] Eingabe des gewünschten Computernamens integrieren.
- [ ] Optionale Eingabe der Gerätenummer für OEM-Informationen integrieren.
- [ ] Bereits vorhandene Gerätenummer aus den vorgesehenen OEM-Informationen auslesen und in der GUI vorbelegen.
- [ ] Bereits installierte Software zuverlässig erkennen und die zugehörigen Auswahlfelder automatisch aktivieren.
- [ ] Bereits erfolgreich abgeschlossene Treiberinstallation über Systemzustand und/oder projektspezifischen Workflow-Marker nachvollziehen.
- [ ] Softwareauswahl mit den definierten Standardwerten integrieren, wobei erkannter Ist-Zustand Vorrang vor Erststart-Defaults hat.
- [ ] Eingaben validieren und als zentralen Setup-Zustand bereitstellen.

### Akzeptanzkriterien

- Beim ersten Start zeigt das Feld für den Computernamen den aktuell auf dem System gesetzten Computernamen an.
- Vorausgewählt sind bei einem Erstlauf Adobe Reader, Google Chrome und 7-Zip, sofern der erkannte Ist-Zustand keine abweichende Darstellung erfordert.
- Optional auswählbar sind Firefox, Thunderbird, Office 365, G Data Anti Virus / Internet Security / Total Protection und G Data MES Client.
- Bereits installierte unterstützte Software wird bei erneutem Start erkannt und automatisch als ausgewählt dargestellt.
- Eine bereits vorhandene Gerätenummer wird aus den OEM-Informationen gelesen und automatisch in der GUI angezeigt.
- Ein leeres Gerätenummernfeld ist gültig und führt nicht zum Schreiben oder Verändern von OEM-Informationen.
- Wird bei einem späteren Lauf eine Gerätenummer ergänzt, kann ausschließlich der dafür erforderliche OEM-Schritt nachgeholt werden, ohne bereits abgeschlossene unabhängige Schritte zwangsweise erneut auszuführen.
- Der Status einer bereits abgeschlossenen Treiberinstallation kann bei erneutem Start nachvollzogen werden.
- Ungültige Eingaben verhindern den Start der eigentlichen Installation mit verständlicher Rückmeldung.
- Die erfassten und erkannten Werte stehen allen nachfolgenden Schritten konsistent zur Verfügung.

## 4.5 Computername und OEM-Informationen

- [ ] Computernamen validieren und nur bei tatsächlicher Änderung setzen.
- [ ] OEM-Felder und deren Quellen verbindlich definieren.
- [ ] Vorhandene OEM-Gerätenummer zuverlässig auslesen.
- [ ] Hersteller, Gerätenummer, Supportinformationen und weitere definierte OEM-Daten nur dann setzen, wenn eine Gerätenummer angegeben ist.
- [ ] Verhalten für einen später nachgeholten OEM-Schreibvorgang nach bereits abgeschlossenem Hauptworkflow implementieren.
- [ ] Neustartbedarf über das zentrale Fortsetzungsmodell behandeln.

### Akzeptanzkriterien

- Der gewünschte Computername ist nach Abschluss aktiv; ist er unverändert, wird keine unnötige Umbenennung ausgelöst.
- Ohne eingegebene Gerätenummer werden keine OEM-Informationen geschrieben oder verändert.
- Mit eingegebener Gerätenummer werden die festgelegten OEM-Informationen an den vorgesehenen Windows-Stellen korrekt angezeigt.
- Eine bereits vorhandene Gerätenummer kann bei erneutem Start zuverlässig ausgelesen werden.
- Eine später ergänzte Gerätenummer kann die OEM-Informationen nachtragen, ohne bereits erfolgreich abgeschlossene unabhängige Installationsschritte erneut auszuführen.
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
2. aktuelle Primärquellen für die in Frage kommenden Windows-11-Ausführungs-, Deployment- und Testtechnologien prüfen,
3. geeignete Implementierungstechnologie(n), Testarchitektur und das minimale Einzeiler-Bootstrap-/Entry-Point-Konzept festlegen,
4. erst danach einen defensiven `.ps1`-Patch für dessen Implementierung erstellen,
5. praktisch testen,
6. anschließend erst die zugehörigen Roadmap-Punkte als `[x]` markieren.