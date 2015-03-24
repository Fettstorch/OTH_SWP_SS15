# -*- encoding: utf-8 -*-

Gem::Specification.new do |s|
  s.name = "password"
  s.version = "1.3"

  s.required_rubygems_version = Gem::Requirement.new(">= 0") if s.respond_to? :required_rubygems_version=
  s.authors = ["Giles Bowkett"]
  s.date = "2008-10-06"
  s.email = "gilesb@gmail.com"
  s.executables = ["password"]
  s.files = ["bin/password"]
  s.homepage = "http://gilesbowkett.blogspot.com/2008/02/sudo-gem-install-password.html"
  s.require_paths = ["lib"]
  s.rubyforge_project = "password"
  s.rubygems_version = "2.0.3"
  s.summary = "Brain-dead simple password storage."

  if s.respond_to? :specification_version then
    s.specification_version = 2

    if Gem::Version.new(Gem::VERSION) >= Gem::Version.new('1.2.0') then
      s.add_runtime_dependency(%q<activesupport>, [">= 2.0.2"])
    else
      s.add_dependency(%q<activesupport>, [">= 2.0.2"])
    end
  else
    s.add_dependency(%q<activesupport>, [">= 2.0.2"])
  end
end
