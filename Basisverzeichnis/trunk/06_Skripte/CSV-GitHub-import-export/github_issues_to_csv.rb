#!/usr/bin/ruby

# type 'gem install octokit' for installation
require 'octokit'
require 'csv'
require 'date'
require 'io/console'

OpenSSL::SSL::VERIFY_PEER = OpenSSL::SSL::VERIFY_NONE

# BEGIN HARD-CODED SECTION
# Un-comment out this section (from here down to where the end is marked) if you want to use this without any interaction
# All of these need to be filled out in order for it to work


arguments=ARGV[0].split(" ")

OUTPUT_FILE = arguments[3]
# puts "User:"
USERNAME = arguments[0]
puts "Enter Password: "
PASSWORD = arguments[4]
if PASSWORD == nil
	PASSWORD = STDIN.noecho(&:gets).chomp
end
ORG = arguments[1]        # Put your organization (or username if you have no org) name here
REPO = arguments[2]       # Put the repository name here
# Want to only get a single milestone? Put the milestone name in here:
TARGET_MILESTONE="" # keep this equal to "" if you want all milestones


puts "User:" + USERNAME
# puts "PW:" + PASSWORD
puts "Organization:" + ORG
puts "Repository:" + REPO
puts TARGET_MILESTONE

# END HARD-CODED SECTION


# Your local timezone offset to convert times
TIMEZONE_OFFSET="-4"
 
client = Octokit::Client.new(:login => USERNAME, :password => PASSWORD)
 
csv = CSV.new(File.open(OUTPUT_FILE + ".txt", 'w'))
 
puts "Initialising CSV file..."
#CSV Headers
header = [
  "Issue number",
  "Title",
  "Description",
  "Date created",
  "Date modified",
  "Labels",
  "Milestone",
  "Status",
  "Assignee",
  "Reporter"
]

csv << header
 
puts "Getting issues from Github..."
temp_issues = []
issues = []
page = 0
begin
	page = page +1
	temp_issues = client.list_issues("#{ORG}/#{REPO}", :state => "closed", :page => page)
	issues = issues + temp_issues;
end while not temp_issues.empty?
temp_issues = []
page = 0
begin
	page = page +1
	temp_issues = client.list_issues("#{ORG}/#{REPO}", :state => "open", :page => page)
	issues = issues + temp_issues;
end while not temp_issues.empty?
 
puts "Processing #{issues.size} issues..."
issues.each do |issue|

	labels = ""
	label = issue['labels'] || "None"
	if (label != "None")
		label.each do |item|
    		labels += item['name'] + " " 
    	end	
	end

	assignee = ""
	assignee = issue['assignee'] || "None"
	if (assignee != "None")
		assignee = assignee['login']
	end	

	milestone = issue['milestone'] || "None"
	if (milestone != "None")
		milestone = milestone['title']
	end
 
	if ((TARGET_MILESTONE == "") || (milestone == TARGET_MILESTONE))
    # Needs to match the header order above, date format are based on Jira default
    row = [
      issue['number'],
      issue['title'],
      issue['body'],
      DateTime.parse(issue['created_at'].to_s).new_offset(TIMEZONE_OFFSET).strftime("%d/%b/%y %l:%M %p"),
      DateTime.parse(issue['updated_at'].to_s).new_offset(TIMEZONE_OFFSET).strftime("%d/%b/%y %l:%M %p"),
      labels,
      milestone,
      issue['state'],
      assignee,
      issue['user']['login']
    ]
    csv << row
    end
end

puts "Done!"
