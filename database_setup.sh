#!/bin/bash

# Define the folder path where SQL files are located
SQL_FOLDER="C:\Users\SimTa\Documents\Boucher_Double\ScriptPostGres"

# Define the database name
DATABASE_NAME="boucher_double"
PASSWORD="Asczdvefb1"

# Function to create the database
create_database() {
  echo "Creating database..."
  PGPASSWORD="$PASSWORD" psql -U postgres -c "DROP DATABASE $DATABASE_NAME;"
  PGPASSWORD="$PASSWORD" psql -U postgres -c "CREATE DATABASE $DATABASE_NAME;"
  echo "Database created successfully!"
}

# Function to execute SQL files
execute_sql_files() {
  local file_name=$1
  local action=$2

  echo "Executing $file_type..."

  # Find all the SQL files in the folder
  find "$SQL_FOLDER" -name "$file_name.sql" | while read -r file; do
    echo "Executing file: $file"
    PGPASSWORD="$PASSWORD" psql -U postgres -d $DATABASE_NAME -f "$file"
  done

  echo "$file_type executed successfully!"
}

# Function to add a function
add_function() {
  execute_sql_files "function" "Adding function"
}

# Function to add a trigger
add_trigger() {
  execute_sql_files "trigger" "Adding trigger"
}

# Function to add a view
add_view() {
  execute_sql_files "views" "Adding view"
}

add_table() {
  execute_sql_files "create" "Adding view"
}

# Main script logic
create_database
add_table
add_function
add_trigger
add_view

echo "Script execution completed."
