-- RepVector Seeder Script
-- This script populates the database with initial testing data.

SET FOREIGN_KEY_CHECKS = 0;

-- 1. Muscle Groups
INSERT IGNORE INTO muscle_groups (name) VALUES 
('Abdominals'), ('Abductors'), ('Biceps'), ('Calves'), ('Cardio'), 
('Chest'), ('Forearms'), ('Full Body'), ('Glutes'), ('Hamstrings'), 
('Lats'), ('Lower Back'), ('Neck'), ('Quadriceps'), ('Shoulders'), 
('Traps'), ('Upper Back'), ('Other');

-- 2. Predefined Exercises (Master Library)
INSERT IGNORE INTO exercises (name, description, type, primary_muscle_group_id, is_predefined) VALUES 
('Bench Press', 'Classic chest exercise for building mass.', 'Barbell', (SELECT id FROM muscle_groups WHERE name = 'Chest'), 1),
('Squat', 'The king of leg exercises.', 'Barbell', (SELECT id FROM muscle_groups WHERE name = 'Quadriceps'), 1),
('Deadlift', 'Foundational compound movement for the posterior chain.', 'Barbell', (SELECT id FROM muscle_groups WHERE name = 'Lower Back'), 1),
('Pull-Up', 'Upper body pulling movement.', 'Bodyweight', (SELECT id FROM muscle_groups WHERE name = 'Lats'), 1),
('Overhead Press', 'Strict shoulder press.', 'Barbell', (SELECT id FROM muscle_groups WHERE name = 'Shoulders'), 1),
('Dumbbell Curl', 'Isolation exercise for the biceps.', 'Dumbell', (SELECT id FROM muscle_groups WHERE name = 'Biceps'), 1),
('Leg Press', 'Machine-based leg training.', 'Machine', (SELECT id FROM muscle_groups WHERE name = 'Quadriceps'), 1),
('Face Pulls', 'Resistance band or cable exercise for rear delts and traps.', 'Resistance Band', (SELECT id FROM muscle_groups WHERE name = 'Shoulders'), 1),
('Kettlebell Swing', 'Explosive hip hinge movement.', 'Kettlebell', (SELECT id FROM muscle_groups WHERE name = 'Glutes'), 1),
('Dips', 'Compound triceps and chest movement.', 'Bodyweight', (SELECT id FROM muscle_groups WHERE name = 'Chest'), 1);

-- 3. Seed Users (Password: Password123!)
SET @common_hash = 'AQIDBAUGBwgJCgsMDQ4PENgxkti7kZ43fC/BNWkOeQWEVtRPychGoUSxbEwraWG8';

INSERT IGNORE INTO users (email, password_hash, role) VALUES 
('admin@repvector.com', @common_hash, 'Admin'),
('user1@repvector.com', @common_hash, 'User'),
('user2@repvector.com', @common_hash, 'User'),
('user3@repvector.com', @common_hash, 'User'),
('user4@repvector.com', @common_hash, 'User'),
('user5@repvector.com', @common_hash, 'User');

-- 4. User Preferences for seeded users
INSERT IGNORE INTO user_preferences (user_id, username, weight_unit, distance_unit)
SELECT id, SUBSTRING_INDEX(email, '@', 1), 'KG', 'KM' FROM users;

SET FOREIGN_KEY_CHECKS = 1;
