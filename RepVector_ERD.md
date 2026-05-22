Note: 
In Visual Studio Code 1.121.0 and later, you can view this mermaid diagram directly in Visual Studio code. A community extention for viewing was intergrated into visual studio itself.


```Mermaid
  erDiagram
      USERS ||--|| USER_PREFERENCES : "has"
      USERS ||--o{ EXERCISES : "creates custom"
      USERS ||--o{ WORKOUTS : "creates custom"
      USERS ||--o{ WORKOUT_SESSIONS : "performs"
  
      MUSCLE_GROUPS ||--o{ EXERCISES : "targeted by"
  
      WORKOUTS ||--o{ WORKOUT_EXERCISES : "defines"
       EXERCISES ||--o{ WORKOUT_EXERCISES : "included in"
   
       WORKOUT_SESSIONS ||--o{ WORKOUT_SET_LOGS : "records"
       EXERCISES ||--o{ WORKOUT_SET_LOGS : "performed in"
       WORKOUTS |o--o{ WORKOUT_SESSIONS : "template for"
   
       USERS {
           int id PK "NN"
           string email "NN"
           string password_hash "NN"
           datetime created_at "NN"
           enum role "NN"
       }
   
       USER_PREFERENCES {
           int user_id PK, FK "NN"
           string username "NULL"
           enum weight_unit "NULL"
           enum distance_unit "NULL"
           datetime updated_at "NULL"
       }
   
       MUSCLE_GROUPS {
           int id PK "NN"
           string name "NN"
       }
   
       EXERCISES {
           int id PK "NN"
           int user_id FK "NULL"
           string name "NN"
           string description "NULL"
           enum type "NN"
           int primary_muscle_group_id FK "NULL"
           boolean is_predefined "NN"
       }
   
       WORKOUTS {
           int id PK "NN"
           int user_id FK "NULL"
           string name "NN"
           string description "NULL"
           boolean is_predefined "NN"
           datetime created_at "NN"
       }
   
       WORKOUT_EXERCISES {
           int id PK "NN"
           int workout_id FK "NN"
           int exercise_id FK "NN"
           int target_sets "NN"
           int target_reps "NN"
           int target_rpe "NULL"
           int sort_order "NN"
       }
   
       WORKOUT_SESSIONS {
           int id PK "NN"
           int user_id FK "NN"
           int workout_id FK "NULL"
           string workout_name "NULL"
           datetime started_at "NN"
           datetime finished_at "NULL"
           int total_seconds "NULL"
           enum status "NULL"
       }
   
       WORKOUT_SET_LOGS {
           int id PK "NN"
           int session_id FK "NN"
           int exercise_id FK "NN"
           int set_number "NN"
           decimal weight "NULL"
           int reps "NN"
           int rpe "NULL"
           enum set_type "NULL"
       }