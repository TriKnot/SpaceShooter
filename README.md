**The release git commits may be missing some assets, they are uploaded separately and can be copy pasted into the folder structure overwriting everything**

# Space Shooter for Computer Technology Course

**4-week Assignment in Computer Technology Course at FutureGames**

I developed a 3D asteroid game in Unity. While it resembles a tech demo more than a fully-fledged game, its purpose was to stress hardware capabilities and practice analyzing performance. I created three different versions, although there's a fourth that is unfinished by the deadline.

## Version 1.0 - No Optimization
The initial version was created with no optimization considerations. There was no object pooling, and each GameObject had its own Update/FixedUpdate scripts.

## Version 1.1 - Pooling
The second version builds upon the first and introduces object pooling. However, no other major changes or optimizations were made.

## Version 1.2 - Jobs Movement
The third version incorporates Unity Burst and Jobs for asteroid movement. Additionally, this version includes a main menu with settings for easy testing of different approaches within the same build.

## Version 1.3 - ECS
I began exploring an implementation with Unity Entities, but I didn't have enough time to complete it before the deadline. I will continue working on it so it might be up soon-ish..

# Performance Analysis
Note: I did not manage to get away from VSync even though I turned it off both in the editor and through script. The only explanation I could find online about it is that it is VSync on my GPU, and I could not find a way to turn it off. I have included VSync in the stats where it is a relevant part of the frame time.

## 5,000 Asteroids

### Version 1.0 - No Optimization (5,000 Asteroids):
FixedUpdate: 4.43ms (27.6%)
Physics: 3.29ms (20.5%)
VSync: 6.15ms (38.4%)

### Version 1.1 - Pooling (5,000 Asteroids):
No significant difference in CPU time compared to Version 1.0. It was expected that object instantiation and destruction might have an impact during runtime, but this wasn't evident in the profiler. The only time I found a difference was in the setup of the scene where I spawned in the bulk of the asteroids, where it was a lot faster at populating the scene.

### Version 1.2 - Jobs/Burst (5,000 Asteroids):
FixedUpdate: 0.9ms (4.2%)
Physics: 4.2ms (19.6%)
VSync: 12.91ms (60.2%)
Jobs: 0.16ms

### Analysis
Surprisingly, there was barely any difference between Versions 1.0 and 1.1. The variance introduced by pooling might be more noticeable on lower-spec machines, although I don't have the means to test that currently. The substantial difference emerged when comparing Version 1.0 and Version 1.2, where the CPU time for movement decreased significantly, dropping from approximately 27.6% to around 4.2%. This reduction is substantial and indicates the remarkable performance benefits of a more data-oriented approach. To optimize my time and resources, I will focus my profiling efforts solely on the pooling and jobs versions when dealing with larger quantities of asteroids.

## 10,000 Asteroids

### Version 1.1 - Pooling (10,000 Asteroids):
FixedUpdate: 7.65ms (37.5%)
Physics: 4.56ms (22.4%)
VSync: 6.39ms (31.3%)

### Version 1.2 - Jobs/Burst (10,000 Asteroids):
FixedUpdate: 0.41ms (3.3%)
Physics: 5.07ms (40.4%)
VSync: 5.35ms (41.7%)
Jobs: 0.38ms

### Analysis
The key takeaway from this test is the significant difference in the FixedUpdate section. Version 1.1 experienced a substantial increase in FixedUpdate time, while the Jobs/Burst version maintained very low overall CPU time. The percentage even went down overall as other factors increased by comparison. However, despite these improvements, physics and VSync still remain resource-intensive. Further optimization in these areas could lead to more balanced performance.

## 15,000 Asteroids

### Version 1.1 - Pooling (15,000 Asteroids):
FixedUpdate: 11.93ms + 9.58ms (55.5%)
Physics: 7.73ms + 7.15ms (38.6%)

### Version 1.2 - Jobs/Burst (15,000 Asteroids):
FixedUpdate: <1%
Physics: 7.8ms (46.2%)
VSync: 6.40ms (67.9%)
Jobs: 0.47ms

### Analysis
A similar pattern emerges with 15,000 asteroids. In Version 1.1, FixedUpdate had to tick twice over each frame as the FPS went down so low. In contrast, it is now so low on the 1.2 version that it barely even makes the charts.

## 50,000 Asteroids

### Version 1.1 - Pooling
Skipped as it's not feasible.

### Version 1.2 - Jobs/Burst (Physics on)
Performance was choppy with <3 FPS. Not playable at all.

### Version 1.2 - Jobs/Burst (Physics off)
Performance was choppy with <3 FPS when looking at the asteroids. However, when looking away, it reached around 100 FPS.

### Analysis
In the context of 50,000 asteroids, the assessment indicates that the performance is challenging for both Version 1.1 (Pooling) and Version 1.2 (Jobs/Burst). There are important distinctions, with Version 1.1 barely achieving a single frame, leading to its omission from further profiling. The performance gap between having physics enabled, rendering the asteroids, and not rendering them clearly demonstrates that movement with Burst/Jobs is working very well while other areas need further optimization.

# Summary
This project has confirmed the advantages of a data-oriented design. Separating data from objects significantly improved performance. Initially, the physics and FixedUpdate movement had similar performance. Still, after implementing Jobs, the movement had minimal impact on the profiler.

The graphics and physics are the next areas for improvement, as evidenced by tests with 50,000 asteroids with and without physics and while looking at them. Looking away from the asteroids provided a better experience.

The next step involves experimenting with the ECS (Entity Component System) using the same assets. Initial tests with asteroids and trigger collisions showed promising performance gains, but graphics performance needs further attention. Improved graphic assets could lead to better overall performance, even though I'm not an artist.
