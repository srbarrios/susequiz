package com.suse.hackweek.quiz.services;

import java.util.List;

import javax.ws.rs.NotFoundException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.data.redis.core.ValueOperations;
import org.springframework.stereotype.Service;

import com.suse.hackweek.quiz.entities.User;

@Service
public class UserService {

	@Autowired
    private RedisTemplate<String, User> userTemplate;

	private static final String REDIS_PREFIX_USERS = "users";

	private static final String REDIS_KEYS_SEPARATOR = ":";

	public List<User> findByPattern(final String pattern) {
		return getValueOperations().multiGet(userTemplate.keys(getRedisKey(pattern)));
	}

	public User findByMailAddress(final String mailAddress) {
		final User user = getValueOperations().get(getRedisKey(mailAddress));
		if(user == null) {
			throw new NotFoundException("User does not exist in the DB");
		}
		return user;
	}

	public void save(final User user) {
		getValueOperations().set(getRedisKey(user.getMailAddress()), user);
	}

	public void update(final User user) {
		findByMailAddress(user.getMailAddress());
		getValueOperations().set(getRedisKey(user.getMailAddress()), user);
	}

	public void delete(final String mailAddress) {
		if(!userTemplate.delete(getRedisKey(mailAddress))) {
			throw new NotFoundException("User does not exist in the DB");
		}
	}

	private String getRedisKey(final String userId) {
        return REDIS_PREFIX_USERS + REDIS_KEYS_SEPARATOR + userId;
    }

	private ValueOperations<String, User> getValueOperations() {
		return userTemplate.opsForValue();
	}

}
