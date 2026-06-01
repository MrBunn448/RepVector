-- RepVector Database Blueprint
-- Database: i572013_repvectorprod
-- This script creates the table structure only.

SET FOREIGN_KEY_CHECKS = 0;

-- 1. Users & Preferences
CREATE TABLE IF NOT EXISTS users (
  id INT NOT NULL AUTO_INCREMENT,
  email VARCHAR(255) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  role ENUM('User', 'Admin') NOT NULL DEFAULT 'User',
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS user_preferences (
  user_id INT PRIMARY KEY,
  username VARCHAR(50),
  weight_unit ENUM('KG', 'LBS') DEFAULT 'KG',
  distance_unit ENUM('KM', 'Miles') DEFAULT 'KM',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_prefs_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. Muscle Groups (Admin Managed)
CREATE TABLE IF NOT EXISTS muscle_groups (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(50) NOT NULL UNIQUE,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Exercises (Master Library & Personal)
CREATE TABLE IF NOT EXISTS exercises (
  id INT NOT NULL AUTO_INCREMENT,
  user_id INT NULL,
  name VARCHAR(100) NOT NULL,
  description TEXT,
  type ENUM('Machine', 'Bodyweight', 'Barbell', 'Dumbell', 'Kettlebell', 'Plate', 'Resistance Band', 'Suspension Band', 'Other') NOT NULL DEFAULT 'Other',
  primary_muscle_group_id INT NULL,
  is_predefined TINYINT(1) NOT NULL DEFAULT 0,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  CONSTRAINT fk_ex_muscle FOREIGN KEY (primary_muscle_group_id) REFERENCES muscle_groups(id) ON DELETE SET NULL,
  CONSTRAINT fk_ex_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Workouts (Templates)
CREATE TABLE IF NOT EXISTS workouts (
  id INT NOT NULL AUTO_INCREMENT,
  user_id INT NULL,
  name VARCHAR(100) NOT NULL,
  description TEXT,
  is_predefined TINYINT(1) NOT NULL DEFAULT 0,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  CONSTRAINT fk_workouts_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5. Workout-Exercise Mapping (Static Template definition)
CREATE TABLE IF NOT EXISTS workout_exercises (
  id INT NOT NULL AUTO_INCREMENT,
  workout_id INT NOT NULL,
  exercise_id INT NOT NULL,
  target_sets INT NOT NULL DEFAULT 3,
  target_reps INT NOT NULL DEFAULT 10,
  target_rpe INT NULL,
  sort_order INT NOT NULL DEFAULT 0,
  PRIMARY KEY (id),
  CONSTRAINT fk_we_workout FOREIGN KEY (workout_id) REFERENCES workouts (id) ON DELETE CASCADE,
  CONSTRAINT fk_we_exercise FOREIGN KEY (exercise_id) REFERENCES exercises (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 6. Workout Sessions (The "Logging" Instance)
CREATE TABLE IF NOT EXISTS workout_sessions (
  id INT NOT NULL AUTO_INCREMENT,
  user_id INT NOT NULL,
  workout_id INT NULL,
  workout_name VARCHAR(100),
  started_at DATETIME NOT NULL,
  finished_at DATETIME NULL,
  total_seconds INT DEFAULT 0,
  status ENUM('active', 'paused', 'completed', 'cancelled') DEFAULT 'active',
  PRIMARY KEY (id),
  CONSTRAINT fk_ws_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  CONSTRAINT fk_ws_workout FOREIGN KEY (workout_id) REFERENCES workouts(id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7. Workout Set Logs (The actual performance data)
CREATE TABLE IF NOT EXISTS workout_set_logs (
  id INT NOT NULL AUTO_INCREMENT,
  session_id INT NOT NULL,
  exercise_id INT NOT NULL,
  set_number INT NOT NULL,
  weight DECIMAL(6,2) DEFAULT 0,
  reps INT NOT NULL,
  rpe INT NULL,
  set_type ENUM('Warm-up', 'Normal', 'Failure', 'Drop set') DEFAULT 'Normal',
  completed_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  CONSTRAINT fk_wsl_session FOREIGN KEY (session_id) REFERENCES workout_sessions(id) ON DELETE CASCADE,
  CONSTRAINT fk_wsl_exercise FOREIGN KEY (exercise_id) REFERENCES exercises(id) ON DELETE CASCADE,
  CONSTRAINT chk_rpe CHECK (rpe IS NULL OR (rpe >= 1 AND rpe <= 10))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SET FOREIGN_KEY_CHECKS = 1;
