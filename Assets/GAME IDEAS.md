# GDD & PLANIFICATION - IEM SHOOTER (PROJET SHADER)

## 1. CONTEXTE & OBJECTIFS
**Type :** Arena Shooter (Type Brotato/Vampire Survivors)
**Moteur :** Unity
**Développeur :** Solo (Dev focus, Low Art skills)
**Deadline :** 19 Janvier (4 semaines)
**Objectif Principal (TP) :** Mettre en valeur le "Hologram Shader".

## 2. GAME DESIGN SIMPLIFIÉ
### Pitch
Le joueur est un programme antivirus (ou un drone) piégé dans une simulation. Une IA centrale (Le Boss) apparaît sous forme d'Hologramme géant au centre (ou en fond) de l'arène. Elle envoie des vagues de glitchs (ennemis) pour détruire le joueur.

### Mécaniques Cœurs (The 3 Cs)
*   **Camera :** Top-Down fixe ou léger suivi.
*   **Character :** Déplacement ZQSD, Tir automatique (façon Brotato) ou visée souris.
*   **Controls :** Simple. Dash sur Espace.

### Utilisation du Shader (CRITIQUE POUR LA NOTE)
Le Shader Hologramme sera appliqué sur le **BOSS (IA)** qui observe le joueur.
*   **Interaction 1 (Temps) :** L'hologramme a un effet de "scan" qui passe en boucle (paramètre animé).
*   **Interaction 2 (Gameplay → Shader) :**
    *   Quand le joueur tue des ennemis, l'hologramme du Boss "glitch" (intensité du bruit augmente).
    *   Quand le Boss prend des dégâts (Wave 5), sa couleur change (Bleu -> Rouge) et sa transparence varie.
*   **Interaction 3 (C#) :** Un script `HologramController.cs` modifiera les propriétés du matérial (`_GlitchIntensity`, `_HoloColor`, `_ScanlineSpeed`) en temps réel.

## 3. DIRECTION ARTISTIQUE (LOW COST)
*   **Style :** Cyberpunk abstrait / Tron-like. Fond sombre, néons lumineux.
*   **Ennemis :** Formes géométriques simples (Cubes, Pyramides) avec des couleurs émissives. Pas de textures complexes.
*   **Animations :**
    *   *Pas d'animation d'os.*
    *   Utilisation de "Tweening" (code) pour faire flotter les ennemis.
    *   Utilisation du Shader pour déformer les ennemis (Vertex Displacement).

## 4. PLANIFICATION (SPRINTS)
*Estimation : 10-15h de travail par semaine.*

### SEMAINE 1 : Intégration Shader & Boucle de base (PRIORITÉ MAX)
*Objectif : Avoir le shader fonctionnel et pilotable par code.*
- [ ] **Tech :** Créer le script `HologramController` qui référence le Material du Boss.
- [ ] **Tech :** Exposer les paramètres du Shader (Color, Glitch, Transparency) pour qu'ils soient accessibles en C#.
- [ ] **Art/Tech :** Mettre en place le modèle du Boss (même une sphère ou un modèle gratuit simple) avec le Shader.
- [ ] **Gameplay :** Faire en sorte que le Boss réagisse à une touche clavier (ex: Appuyer sur 'K' fait glitcher le boss) -> *Preuve de concept pour le TP.*

### SEMAINE 2 : Ennemis & Spawners (Les "Halos")
*Objectif : Remplir l'arène.*
- [ ] **Art :** Créer le visuel du Spawner (Cône de lumière avec cercles concentriques - peut réutiliser une variante du shader hologramme !).
- [ ] **Gameplay :** Coder le `WaveManager`.
- [ ] **Gameplay :** Créer 2 types d'ennemis (ex: "Le Fonceur" et "Le Tireur").
- [ ] **Anim :** Faire descendre les ennemis du halo (Tween simple Y axis).

### SEMAINE 3 : Game Loop & Boss Fight
*Objectif : Rendre le jeu jouable du début à la fin.*
- [ ] **Gameplay :** Logique de fin de vague.
- [ ] **Gameplay :** Comportement du Boss en Wave 5 (Il ne fait plus juste regarder, il lance des projectiles).
- [ ] **UI :** Barre de vie du joueur, Compteur de vague.
- [ ] **Tech :** Lier la vie du Boss à l'intensité du Shader (Plus il meurt, plus il devient instable/rouge).

### SEMAINE 4 : Polish & Rendu
*Objectif : "Effet Wow" et nettoyage.*
- [ ] **Audio :** Ajouter musique et SFX (Tir, Impact, Glitch).
- [ ] **VFX :** Particules d'explosion (Unity Particle System standard).
- [ ] **Post-Process :** Bloom (très important pour l'effet néon/hologramme).
- [ ] **Build :** Tester le build WebGL ou Windows.
- [ ] **Rendu :** Préparer la vidéo de démo et le lien Git.

## 5. LISTE DES TÂCHES TECHNIQUES (Backlog)
### Shader & VFX
1.  Finaliser le Shader Graph Hologramme (Ajouter Scanlines, Fresnel, Noise).
2.  Créer un Shader "Dissolve" pour la mort des ennemis (Bonus).
3.  Configurer le Post-Processing (Bloom, Vignette).

### Scripts C# à faire
1.  `HologramParamController.cs` : Interface entre le jeu et le shader.
2.  `WaveManager.cs` : Gère le timing et les spawns.
3.  `EnemyAI.cs` : Déplacement vers le joueur (NavMesh ou simple `Vector3.MoveTowards`).
4.  `BossBehaviour.cs` : États du boss (Idle, Attack, Damaged).

### Assets (Modèles)
*   Utiliser des primitives Unity ou ProBuilder pour l'instant.
*   Chercher un pack "Low Poly SciFi" gratuit sur l'Asset Store si besoin de gagner du temps.

