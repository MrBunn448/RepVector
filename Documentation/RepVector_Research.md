# Research Document  
## Using a REST API Backend for RepVector

---

## Title Page

**Title**  
Evaluating the Use of a REST API Backend for RepVector

**Student Name**  
Giovanni Schmidt  
**Student Number**  
[to be filled in]

**Project Name**  
RepVector, Workout Tracking Application

**Semester and Learning Outcome**  
Semester 2 – Learning Outcome 5 (Professional Standard)

**Date of latests revision**  
[08/06/2026]

---

## 1. Introduction

RepVector is a Workout tracking application developed using .NET 10.0 and a decoupled N-tier architecture. The system is designed to support athletes and gym-goers by enabling structured workout creation, live training session logging, and eventually long-term progress analysis. A key architectural decision in this project is the use of a REST API as the backend interface between the core application logic and the user interface.

The primary problem addressed in this research is determining whether a REST API is the most suitable backend approach to support RepVector’s current requirements while remaining flexible for future expansion, particularly the potential development of a mobile user interface. This decision has significant implications for maintainability, scalability, performance, and long-term development cost.

This problem cannot be solved through intuition or assumptions alone. Architectural choices made early in a project are difficult and (Time)costly to reverse, and subjective preferences for certain technologies may lead to suboptimal outcomes. A structured research approach is therefore necessary to evaluate REST APIs against project-specific criteria such as decoupling, extensibility, performance overhead, and client compatibility.

This research document is explicitly decision-oriented. Its goal is not only to describe REST APIs in general, but to evaluate their suitability for RepVector and to justify the architectural decision based on evidence, best practices, and professional standards.

---

## 2. Main Research Question (RQ)

**How can a REST API backend best support RepVector’s architectural goals?**

---

## 3. Sub-Research Questions

To answer the main research question, it is divided into the following focused sub-research questions:

- **RQ1.** What architectural advantages does a REST API provide in terms of decoupling backend logic from user interfaces?  
- **RQ2.** How does using a REST API affect scalability and future platform expansion, such as mobile applications?  
- **RQ3.** What performance and security considerations are introduced by using a REST API in a high-performance workout tracking application?  
- **RQ4.** How does a REST API compare to alternative backend approaches for RepVector’s use case?

Each sub-question addresses a specific decision-relevant aspect of the architectural choice and contributes directly to answering the main research question.

---

## 4. Research Strategy and Methodology (DOT Framework)

### DOT Framework Overview Table

| Sub-question | DOT Domain(s) | Method(s) | Why this method fits |
|-------------|--------------|-----------|----------------------|
| RQ1 | Library | Literature study, architectural pattern analysis | Identifies proven architectural principles and best practices |
| RQ2 | Library, Showroom | Case studies, comparative analysis | Evaluates real-world implementations and scalability outcomes |
| RQ3 | Library, Lab | Technical documentation review, performance analysis | Assesses concrete performance and security implications |
| RQ4 | Showroom | Benchmarking and comparison | Enables objective comparison |

### Methodology Explanation

The chosen DOT strategies align with the nature of the research questions. Library research is essential for grounding architectural decisions in established software engineering principles and industry standards. Showroom research supports objective comparison by analyzing how similar systems implement backend architectures. Lab-oriented analysis is used to evaluate performance and security implications at a technical level. Together, these strategies provide reliable, triangulated evidence that directly supports informed architectural decision-making.

---

## 5. Findings

---

### RQ1 – Architectural Decoupling

#### Findings

Research on layered and clean architecture consistently highlights the importance of separating business logic from presentation layers.[^3] REST APIs act as a stable contract between backend services and clients, allowing each to evolve independently. This aligns with RepVector’s existing decoupled N-tier architecture, where the Logic layer remains independent of UI concerns. REST enforces clear boundaries through HTTP-based communication and data transfer objects, reducing coupling and simplifying maintenance.

#### Sub-Conclusion

A REST API significantly improves architectural decoupling by enforcing separation of concerns. This directly supports cleaner code organization and easier long-term maintenance. The backend becomes UI-agnostic, which aligns with RepVector’s architectural principles.

#### Project Implication

RepVector’s Logic and DAL layers can remain unchanged when UI implementations evolve. This allows Razor Pages, mobile apps, or other clients to coexist without duplicating business logic.

---

### RQ2 – Scalability and Future Platform Expansion

#### Findings

Industry case studies show that REST APIs are widely used in systems that support multiple client platforms, including web, mobile, and third-party integrations.[^4] REST’s stateless nature allows horizontal scaling and simplifies load balancing. For RepVector, this means that future mobile applications can reuse the same backend functionality without architectural changes.

#### Mini-Conclusion

REST APIs provide a proven foundation for scalable, multi-platform applications. They allow RepVector to expand beyond its current UI without reworking backend logic or data access layers.

#### Project Implication

The decision to use a REST API directly supports the planned development of a mobile UI while preserving all backend functionality.

---

### RQ3 – Performance and Security Considerations

#### Findings

REST APIs introduce some overhead due to serialization, network latency, and HTTP processing. However, research indicates that with efficient data models, proper caching, and optimized endpoints, this overhead is minimal for applications like RepVector. Security best practices such as stateless authentication, role-based access control, and HTTPS are well-established in REST-based systems.[^2]

#### Sub-Conclusion

While REST APIs introduce measurable overhead, the impact is acceptable and manageable within RepVector’s performance requirements. Security mechanisms are mature and align with professional standards.

#### Project Implication

RepVector can safely use a REST API without compromising performance or security, provided endpoints are carefully designed and optimized.

---

### RQ4 – Comparison with Alternative Approaches

#### Findings

Alternatives such as tightly coupled server-rendered architectures or direct database access from the UI reduce flexibility and increase long-term risk. While they may offer slightly lower latency, they severely limit extensibility and violate separation of concerns. Compared to these approaches, REST provides a balanced trade-off between performance, flexibility, and maintainability.[^1]

#### Sub-Conclusion

REST APIs offer the most appropriate balance for RepVector’s needs when compared to alternative backend approaches.

#### Project Implication

Choosing REST avoids architectural lock-in and supports professional software development standards.

---

## 6. General Conclusion

This research aimed to determine whether a REST API backend is the most suitable architectural choice for RepVector. The findings show that REST APIs strongly support decoupling, scalability, and future platform expansion while maintaining acceptable performance and security characteristics.

Based on the combined evidence from architectural theory, industry practices, and technical analysis, the recommended decision is to use a REST API as the backend interface for RepVector. This decision directly supports the project’s goals of maintainability, extensibility, and professional software design. Remaining risks primarily relate to performance optimization and API versioning, both of which can be mitigated through careful implementation and ongoing monitoring.

---

## 7. Reflection on the Research Process

The research was conducted using a structured DOT framework approach, primarily relying on Library and Showroom strategies. Literature studies and documentation reviews provided a solid theoretical foundation, while comparative analysis helped contextualize the findings within real-world applications.

One aspect that worked well was mapping research questions directly to architectural decisions, which kept the research focused and decision-oriented. A challenge encountered was balancing theoretical best practices with project-specific constraints, particularly performance considerations. Through this process, I learned the importance of explicitly justifying architectural decisions rather than relying on personal preference or familiarity.

In future projects, I would improve this research by incorporating more empirical testing, such as benchmarking different API designs under load, to further strengthen evidence-based decision-making.

---

## 8. References (APA Style)

Fielding, R. T. (2000). *Architectural styles and the design of network-based software architectures* (Doctoral dissertation, University of California, Irvine).

Microsoft. (2024). *ASP.NET Core Web API documentation*.  
https://learn.microsoft.com/aspnet/core/web-api

Martin, R. C. (2017). *Clean architecture: A craftsman’s guide to software structure and design*. Prentice Hall.

Newman, S. (2021). *Building microservices* (2nd ed.). O’Reilly Media.

[^1]: Fielding, R. T. (2000). *Architectural styles and the design of network-based software architectures* (Doctoral dissertation, University of California, Irvine).
[^2]: Microsoft. (2024). *ASP.NET Core Web API documentation*. https://learn.microsoft.com/aspnet/core/web-api
[^3]: Martin, R. C. (2017). *Clean architecture: A craftsman’s guide to software structure and design*. Prentice Hall.
[^4]: Newman, S. (2021). *Building microservices* (2nd ed.). O’Reilly Media.

---
